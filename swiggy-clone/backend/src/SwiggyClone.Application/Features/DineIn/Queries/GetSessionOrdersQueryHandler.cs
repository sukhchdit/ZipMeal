using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Orders.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.DineIn.Queries;

internal sealed class GetSessionOrdersQueryHandler(IAppDbContext db)
    : IRequestHandler<GetSessionOrdersQuery, Result<List<OrderDto>>>
{
    public async Task<Result<List<OrderDto>>> Handle(
        GetSessionOrdersQuery request, CancellationToken ct)
    {
        // 1. Validate user is a member
        var isMember = await db.DineInSessionMembers
            .AnyAsync(m => m.SessionId == request.SessionId
                && m.UserId == request.UserId, ct);

        if (!isMember)
            return Result<List<OrderDto>>.Failure("NOT_SESSION_MEMBER",
                "You are not a member of this dine-in session.");

        // 2. Load all orders for this session
        var orders = await db.Orders
            .AsNoTracking()
            .Where(o => o.DineInSessionId == request.SessionId)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderDto(
                o.Id,
                o.OrderNumber,
                o.RestaurantId,
                o.Restaurant.Name,
                o.OrderType,
                o.Status,
                o.Subtotal,
                o.TaxAmount,
                o.DeliveryFee,
                o.PackagingCharge,
                o.DiscountAmount,
                o.TotalAmount,
                o.PaymentStatus,
                o.PaymentMethod,
                o.SpecialInstructions,
                o.EstimatedDeliveryTime,
                o.ScheduledDeliveryTime,
                o.CreatedAt,
                o.Items.Select(i => new OrderItemDto(
                    i.Id,
                    i.MenuItemId,
                    i.ItemName,
                    i.VariantId,
                    i.Quantity,
                    i.UnitPrice,
                    i.TotalPrice,
                    i.SpecialInstructions,
                    i.Addons.Select(a => new OrderItemAddonDto(
                        a.AddonId, a.AddonName, a.Quantity, a.Price)).ToList()
                )).ToList(),
                false,
                o.TipAmount,
                o.TipAmount > 0))
            .ToListAsync(ct);

        return Result<List<OrderDto>>.Success(orders);
    }
}
