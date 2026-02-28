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

internal sealed class RedisCartService(
    IDistributedCache cache,
    IAppDbContext db,
    ILogger<RedisCartService> logger,
    [FromKeyedServices("redis-cart")] ResiliencePipeline pipeline) : ICartService
{
    private static readonly TimeSpan CartTtl = TimeSpan.FromDays(7);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public async Task<Result<CartDto>> GetCartAsync(Guid userId, CancellationToken ct = default)
    {
        var cart = await LoadCartAsync(userId, ct);
        return cart is null
            ? Result<CartDto>.Success(new CartDto(Guid.Empty, string.Empty, [], 0))
            : Result<CartDto>.Success(cart);
    }

    public async Task<Result<CartDto>> AddToCartAsync(Guid userId, AddToCartItem item, CancellationToken ct = default)
    {
        // Validate menu item exists
        var menuItem = await db.MenuItems
            .AsNoTracking()
            .Include(m => m.Variants)
            .Include(m => m.Addons)
            .FirstOrDefaultAsync(m => m.Id == item.MenuItemId, ct);

        if (menuItem is null)
            return Result<CartDto>.Failure("MENU_ITEM_NOT_FOUND", "Menu item not found.");

        // Validate restaurant
        var restaurant = await db.Restaurants
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == item.RestaurantId, ct);

        if (restaurant is null)
            return Result<CartDto>.Failure("RESTAURANT_NOT_FOUND", "Restaurant not found.");

        var cart = await LoadCartAsync(userId, ct);

        // Guard against different restaurant conflict
        if (cart is not null && cart.Items.Count > 0 && cart.RestaurantId != item.RestaurantId)
            return Result<CartDto>.Failure("DIFFERENT_RESTAURANT",
                "Your cart contains items from a different restaurant. Clear the cart first.");

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

        // Build deterministic cart item ID
        var cartItemId = GenerateCartItemId(item.MenuItemId, item.VariantId, item.Addons);

        var basePrice = menuItem.DiscountedPrice ?? menuItem.Price;
        var unitPrice = basePrice + variantAdjustment + addonsTotal;

        // Merge or add
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

        await SaveCartAsync(userId, updatedCart, ct);
        return Result<CartDto>.Success(updatedCart);
    }

    public async Task<Result<CartDto>> UpdateQuantityAsync(Guid userId, string cartItemId, int quantity, CancellationToken ct = default)
    {
        var cart = await LoadCartAsync(userId, ct);
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

        await SaveCartAsync(userId, updatedCart, ct);
        return Result<CartDto>.Success(updatedCart);
    }

    public async Task<Result<CartDto>> RemoveItemAsync(Guid userId, string cartItemId, CancellationToken ct = default)
    {
        return await UpdateQuantityAsync(userId, cartItemId, 0, ct);
    }

    public async Task<Result> ClearCartAsync(Guid userId, CancellationToken ct = default)
    {
        try
        {
            await pipeline.ExecuteAsync(async token =>
            {
                await cache.RemoveAsync(CartKey(userId), token);
            }, ct);
        }
        catch (BrokenCircuitException)
        {
            logger.LogWarning("Redis circuit open — skipping cart clear for user {UserId}", userId);
        }
        catch (TimeoutRejectedException)
        {
            logger.LogWarning("Redis cart clear timed out for user {UserId}", userId);
        }

        return Result.Success();
    }

    // ───────────────────────── Helpers ─────────────────────────

    private static string CartKey(Guid userId) => $"cart:{userId}";

    private static string GenerateCartItemId(Guid menuItemId, Guid? variantId, List<CartAddonInput> addons)
    {
        var sortedAddonIds = addons.OrderBy(a => a.AddonId).Select(a => a.AddonId.ToString());
        return $"{menuItemId}_{variantId?.ToString() ?? "none"}_{string.Join(",", sortedAddonIds)}";
    }

    private async Task<CartDto?> LoadCartAsync(Guid userId, CancellationToken ct)
    {
        try
        {
            return await pipeline.ExecuteAsync(async token =>
            {
                var json = await cache.GetStringAsync(CartKey(userId), token);
                return json is null ? null : JsonSerializer.Deserialize<CartDto>(json, JsonOptions);
            }, ct);
        }
        catch (BrokenCircuitException)
        {
            logger.LogWarning("Redis circuit open — returning null cart for user {UserId}", userId);
            return null;
        }
        catch (TimeoutRejectedException)
        {
            logger.LogWarning("Redis cart load timed out for user {UserId}", userId);
            return null;
        }
    }

    private async Task SaveCartAsync(Guid userId, CartDto cart, CancellationToken ct)
    {
        try
        {
            await pipeline.ExecuteAsync(async token =>
            {
                var json = JsonSerializer.Serialize(cart, JsonOptions);
                await cache.SetStringAsync(CartKey(userId), json, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CartTtl,
                }, token);
            }, ct);
        }
        catch (BrokenCircuitException)
        {
            logger.LogWarning("Redis circuit open — skipping cart save for user {UserId}", userId);
        }
        catch (TimeoutRejectedException)
        {
            logger.LogWarning("Redis cart save timed out for user {UserId}", userId);
        }
    }
}
