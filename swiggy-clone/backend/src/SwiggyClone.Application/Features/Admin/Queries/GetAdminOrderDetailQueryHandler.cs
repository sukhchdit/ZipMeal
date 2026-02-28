using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Admin.DTOs;
using SwiggyClone.Application.Features.Orders.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Admin.Queries;

internal sealed class GetAdminOrderDetailQueryHandler(IAppDbContext db)
    : IRequestHandler<GetAdminOrderDetailQuery, Result<AdminOrderDetailDto>>
{
    public async Task<Result<AdminOrderDetailDto>> Handle(
        GetAdminOrderDetailQuery request, CancellationToken ct)
    {
        var order = await db.Orders
            .AsNoTracking()
            .AsSplitQuery()
            .Include(o => o.User)
            .Include(o => o.Restaurant)
            .Include(o => o.Items)
                .ThenInclude(i => i.Addons)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);

        if (order is null)
            return Result<AdminOrderDetailDto>.Failure("ORDER_NOT_FOUND", "Order not found.");

        var items = order.Items.Select(i => new OrderItemDto(
            i.Id,
            i.MenuItemId,
            i.ItemName,
            i.VariantId,
            i.Quantity,
            i.UnitPrice,
            i.TotalPrice,
            i.SpecialInstructions,
            i.Addons.Select(a => new OrderItemAddonDto(
                a.AddonId,
                a.AddonName,
                a.Quantity,
                a.Price)).ToList())).ToList();

        return Result<AdminOrderDetailDto>.Success(new AdminOrderDetailDto(
            order.Id,
            order.OrderNumber,
            order.UserId,
            order.User.FullName,
            order.User.PhoneNumber,
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
            order.CancellationReason,
            order.EstimatedDeliveryTime,
            order.ActualDeliveryTime,
            order.CreatedAt,
            items));
    }
}
