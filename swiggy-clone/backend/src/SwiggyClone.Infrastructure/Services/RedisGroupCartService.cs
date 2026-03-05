using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Cart.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Infrastructure.Services;

internal sealed class RedisGroupCartService(
    IDistributedCache cache,
    IAppDbContext db,
    ILogger<RedisGroupCartService> logger,
    [FromKeyedServices("redis-cart")] ResiliencePipeline pipeline) : IGroupCartService
{
    private static readonly TimeSpan CartTtl = TimeSpan.FromHours(2);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public async Task<Result<CartDto>> GetParticipantCartAsync(Guid groupOrderId, Guid userId, CancellationToken ct)
    {
        var cart = await LoadCartAsync(groupOrderId, userId, ct);
        return cart is null
            ? Result<CartDto>.Success(new CartDto(Guid.Empty, string.Empty, [], 0))
            : Result<CartDto>.Success(cart);
    }

    public async Task<Result<CartDto>> AddToCartAsync(Guid groupOrderId, Guid userId, AddToCartItem item, CancellationToken ct)
    {
        var menuItem = await db.MenuItems
            .AsNoTracking()
            .Include(m => m.Variants)
            .Include(m => m.Addons)
            .FirstOrDefaultAsync(m => m.Id == item.MenuItemId, ct);

        if (menuItem is null)
            return Result<CartDto>.Failure("MENU_ITEM_NOT_FOUND", "Menu item not found.");

        var restaurant = await db.Restaurants
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == item.RestaurantId, ct);

        if (restaurant is null)
            return Result<CartDto>.Failure("RESTAURANT_NOT_FOUND", "Restaurant not found.");

        var cart = await LoadCartAsync(groupOrderId, userId, ct);

        // Resolve variant
        string? variantName = null;
        int variantAdjustment = 0;
        if (item.VariantId is not null)
        {
            var variant = menuItem.Variants.FirstOrDefault(v => v.Id == item.VariantId);
            if (variant is not null)
            {
                variantName = variant.Name;
                variantAdjustment = variant.PriceAdjustment;
            }
        }

        // Resolve addons
        var addonDtos = new List<CartItemAddonDto>();
        int addonsTotal = 0;
        foreach (var addonInput in item.Addons)
        {
            var addon = menuItem.Addons.FirstOrDefault(a => a.Id == addonInput.AddonId);
            if (addon is not null)
            {
                addonDtos.Add(new CartItemAddonDto(addon.Id, addon.Name, addon.Price, addonInput.Quantity));
                addonsTotal += addon.Price * addonInput.Quantity;
            }
        }

        var cartItemId = GenerateCartItemId(item.MenuItemId, item.VariantId, item.Addons);
        var basePrice = menuItem.DiscountedPrice ?? menuItem.Price;
        var unitPrice = basePrice + variantAdjustment + addonsTotal;

        var items = cart?.Items.ToList() ?? [];
        var existingIndex = items.FindIndex(i => i.CartItemId == cartItemId);

        if (existingIndex >= 0)
        {
            var existing = items[existingIndex];
            var newQty = existing.Quantity + item.Quantity;
            items[existingIndex] = existing with
            {
                Quantity = newQty,
                TotalPrice = unitPrice * newQty,
            };
        }
        else
        {
            items.Add(new CartItemDto(
                CartItemId: cartItemId,
                MenuItemId: item.MenuItemId,
                VariantId: item.VariantId,
                ItemName: menuItem.Name,
                VariantName: variantName,
                Quantity: item.Quantity,
                UnitPrice: unitPrice,
                TotalPrice: unitPrice * item.Quantity,
                Addons: addonDtos,
                SpecialInstructions: item.SpecialInstructions));
        }

        var subtotal = items.Sum(i => i.TotalPrice);
        var updatedCart = new CartDto(item.RestaurantId, restaurant.Name, items, subtotal);

        await SaveCartAsync(groupOrderId, userId, updatedCart, ct);
        await AddMemberAsync(groupOrderId, userId, ct);
        return Result<CartDto>.Success(updatedCart);
    }

    public async Task<Result<CartDto>> UpdateQuantityAsync(Guid groupOrderId, Guid userId, string cartItemId, int quantity, CancellationToken ct)
    {
        var cart = await LoadCartAsync(groupOrderId, userId, ct);
        if (cart is null || cart.Items.Count == 0)
            return Result<CartDto>.Failure("CART_EMPTY", "Cart is empty.");

        var items = cart.Items.ToList();
        var index = items.FindIndex(i => i.CartItemId == cartItemId);
        if (index < 0)
            return Result<CartDto>.Failure("ITEM_NOT_FOUND", "Cart item not found.");

        if (quantity <= 0)
        {
            items.RemoveAt(index);
        }
        else
        {
            var existing = items[index];
            items[index] = existing with
            {
                Quantity = quantity,
                TotalPrice = existing.UnitPrice * quantity,
            };
        }

        var subtotal = items.Sum(i => i.TotalPrice);
        var updatedCart = items.Count == 0
            ? new CartDto(Guid.Empty, string.Empty, [], 0)
            : cart with { Items = items, Subtotal = subtotal };

        await SaveCartAsync(groupOrderId, userId, updatedCart, ct);
        return Result<CartDto>.Success(updatedCart);
    }

    public async Task<Result<CartDto>> RemoveItemAsync(Guid groupOrderId, Guid userId, string cartItemId, CancellationToken ct)
    {
        return await UpdateQuantityAsync(groupOrderId, userId, cartItemId, 0, ct);
    }

    public async Task<Result> ClearParticipantCartAsync(Guid groupOrderId, Guid userId, CancellationToken ct)
    {
        try
        {
            await pipeline.ExecuteAsync(async token =>
            {
                await cache.RemoveAsync(CartKey(groupOrderId, userId), token);
            }, ct);
        }
        catch (BrokenCircuitException)
        {
            logger.LogWarning("Redis circuit open — skipping group cart clear for {GroupOrderId}/{UserId}", groupOrderId, userId);
        }
        catch (TimeoutRejectedException)
        {
            logger.LogWarning("Redis group cart clear timed out for {GroupOrderId}/{UserId}", groupOrderId, userId);
        }

        return Result.Success();
    }

    public async Task<Result> ClearAllCartsAsync(Guid groupOrderId, CancellationToken ct)
    {
        var members = await GetMembersAsync(groupOrderId, ct);
        foreach (var memberId in members)
        {
            await ClearParticipantCartAsync(groupOrderId, memberId, ct);
        }

        try
        {
            await pipeline.ExecuteAsync(async token =>
            {
                await cache.RemoveAsync(MembersKey(groupOrderId), token);
            }, ct);
        }
        catch (BrokenCircuitException)
        {
            logger.LogWarning("Redis circuit open — skipping group members clear for {GroupOrderId}", groupOrderId);
        }
        catch (TimeoutRejectedException)
        {
            logger.LogWarning("Redis group members clear timed out for {GroupOrderId}", groupOrderId);
        }

        return Result.Success();
    }

    public async Task<List<(Guid UserId, CartDto Cart)>> GetAllParticipantCartsAsync(Guid groupOrderId, CancellationToken ct)
    {
        var members = await GetMembersAsync(groupOrderId, ct);
        var result = new List<(Guid UserId, CartDto Cart)>();

        foreach (var memberId in members)
        {
            var cart = await LoadCartAsync(groupOrderId, memberId, ct);
            if (cart is not null && cart.Items.Count > 0)
            {
                result.Add((memberId, cart));
            }
        }

        return result;
    }

    // ───────────────────────── Helpers ─────────────────────────

    private static string CartKey(Guid groupOrderId, Guid userId) => $"group-cart:{groupOrderId}:{userId}";
    private static string MembersKey(Guid groupOrderId) => $"group-cart:{groupOrderId}:members";

    private static string GenerateCartItemId(Guid menuItemId, Guid? variantId, List<CartAddonInput> addons)
    {
        var sortedAddonIds = addons.OrderBy(a => a.AddonId).Select(a => a.AddonId.ToString());
        return $"{menuItemId}_{variantId?.ToString() ?? "none"}_{string.Join(",", sortedAddonIds)}";
    }

    private async Task<CartDto?> LoadCartAsync(Guid groupOrderId, Guid userId, CancellationToken ct)
    {
        try
        {
            return await pipeline.ExecuteAsync(async token =>
            {
                var json = await cache.GetStringAsync(CartKey(groupOrderId, userId), token);
                return json is null ? null : JsonSerializer.Deserialize<CartDto>(json, JsonOptions);
            }, ct);
        }
        catch (BrokenCircuitException)
        {
            logger.LogWarning("Redis circuit open — returning null group cart for {GroupOrderId}/{UserId}", groupOrderId, userId);
            return null;
        }
        catch (TimeoutRejectedException)
        {
            logger.LogWarning("Redis group cart load timed out for {GroupOrderId}/{UserId}", groupOrderId, userId);
            return null;
        }
    }

    private async Task SaveCartAsync(Guid groupOrderId, Guid userId, CartDto cart, CancellationToken ct)
    {
        try
        {
            await pipeline.ExecuteAsync(async token =>
            {
                var json = JsonSerializer.Serialize(cart, JsonOptions);
                await cache.SetStringAsync(CartKey(groupOrderId, userId), json, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CartTtl,
                }, token);
            }, ct);
        }
        catch (BrokenCircuitException)
        {
            logger.LogWarning("Redis circuit open — skipping group cart save for {GroupOrderId}/{UserId}", groupOrderId, userId);
        }
        catch (TimeoutRejectedException)
        {
            logger.LogWarning("Redis group cart save timed out for {GroupOrderId}/{UserId}", groupOrderId, userId);
        }
    }

    private async Task AddMemberAsync(Guid groupOrderId, Guid userId, CancellationToken ct)
    {
        try
        {
            await pipeline.ExecuteAsync(async token =>
            {
                var key = MembersKey(groupOrderId);
                var existing = await cache.GetStringAsync(key, token);
                var members = existing is null
                    ? new HashSet<string>()
                    : JsonSerializer.Deserialize<HashSet<string>>(existing, JsonOptions) ?? [];
                members.Add(userId.ToString());
                var json = JsonSerializer.Serialize(members, JsonOptions);
                await cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CartTtl,
                }, token);
            }, ct);
        }
        catch (BrokenCircuitException)
        {
            logger.LogWarning("Redis circuit open — skipping member add for {GroupOrderId}", groupOrderId);
        }
        catch (TimeoutRejectedException)
        {
            logger.LogWarning("Redis member add timed out for {GroupOrderId}", groupOrderId);
        }
    }

    private async Task<List<Guid>> GetMembersAsync(Guid groupOrderId, CancellationToken ct)
    {
        try
        {
            return await pipeline.ExecuteAsync(async token =>
            {
                var json = await cache.GetStringAsync(MembersKey(groupOrderId), token);
                if (json is null) return [];
                var members = JsonSerializer.Deserialize<HashSet<string>>(json, JsonOptions);
                return members?.Select(Guid.Parse).ToList() ?? [];
            }, ct);
        }
        catch (BrokenCircuitException)
        {
            logger.LogWarning("Redis circuit open — returning empty members for {GroupOrderId}", groupOrderId);
            return [];
        }
        catch (TimeoutRejectedException)
        {
            logger.LogWarning("Redis members load timed out for {GroupOrderId}", groupOrderId);
            return [];
        }
    }
}
