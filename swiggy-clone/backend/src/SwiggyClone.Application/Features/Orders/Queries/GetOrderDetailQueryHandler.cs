using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Orders.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Orders.Queries;

internal sealed class GetOrderDetailQueryHandler(IAppDbContext db)
    : IRequestHandler<GetOrderDetailQuery, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(GetOrderDetailQuery request, CancellationToken ct)
    {
        var order = await db.Orders
            .AsNoTracking()
            .AsSplitQuery()
            .Include(o => o.Restaurant)
            .Include(o => o.Items)
                .ThenInclude(i => i.Addons)
            .Include(o => o.StatusHistory)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.UserId == request.UserId, ct);

        if (order is null)
            return Result<OrderDto>.Failure("ORDER_NOT_FOUND", "Order not found.");

        var hasReview = await db.Reviews.AnyAsync(r => r.OrderId == order.Id, ct);

        var dto = new OrderDto(
            order.Id,
            order.OrderNumber,
            order.RestaurantId,
            order.Restaurant.Name,
            order.OrderType,
            order.Status,
            order.Subtotal,
            order.TaxAmount,
            order.DeliveryFee,
            order.PackagingCharge,
            order.DiscountAmount,
            order.TotalAmount,
            order.PaymentStatus,
            order.PaymentMethod,
            order.SpecialInstructions,
            order.EstimatedDeliveryTime,
            order.CreatedAt,
            order.Items.Select(i => new OrderItemDto(
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
            hasReview,
            order.TipAmount,
            order.TipAmount > 0);

        return Result<OrderDto>.Success(dto);
    }
}
