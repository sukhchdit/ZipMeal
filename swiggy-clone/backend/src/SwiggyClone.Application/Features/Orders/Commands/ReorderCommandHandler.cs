using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Orders.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Orders.Commands;

internal sealed class ReorderCommandHandler(IAppDbContext db, ICartService cartService)
    : IRequestHandler<ReorderCommand, Result<ReorderResultDto>>
{
    public async Task<Result<ReorderResultDto>> Handle(ReorderCommand request, CancellationToken ct)
    {
        // 1. Fetch order with items + addons
        var order = await db.Orders.AsNoTracking()
            .Include(o => o.Items).ThenInclude(oi => oi.Addons)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.UserId == request.UserId, ct);

        if (order is null)
            return Result<ReorderResultDto>.Failure("ORDER_NOT_FOUND", "Order not found.");

        // 2. Fetch current menu items for validation
        var menuItemIds = order.Items.Select(i => i.MenuItemId).Distinct().ToList();
        var currentMenuItems = await db.MenuItems.AsNoTracking()
            .Where(m => menuItemIds.Contains(m.Id))
            .ToDictionaryAsync(m => m.Id, ct);

        // 3. Clear cart
        await cartService.ClearCartAsync(request.UserId, ct);

        // 4. Process each item
        var unavailable = new List<UnavailableReorderItemDto>();
        foreach (var item in order.Items)
        {
            if (!currentMenuItems.TryGetValue(item.MenuItemId, out var menuItem))
            {
                unavailable.Add(new UnavailableReorderItemDto(item.MenuItemId, item.ItemName, "No longer on the menu"));
                continue;
            }

            if (!menuItem.IsAvailable)
            {
                unavailable.Add(new UnavailableReorderItemDto(item.MenuItemId, item.ItemName, "Currently unavailable"));
                continue;
            }

            await cartService.AddToCartAsync(request.UserId, new AddToCartItem(
                order.RestaurantId,
                item.MenuItemId,
                item.VariantId,
                item.Quantity,
                item.SpecialInstructions,
                item.Addons.Select(a => new CartAddonInput(a.AddonId, a.Quantity)).ToList()), ct);
        }

        // 5. Get final cart
        var cartResult = await cartService.GetCartAsync(request.UserId, ct);
        return Result<ReorderResultDto>.Success(new ReorderResultDto(cartResult.Value, unavailable));
    }
}
