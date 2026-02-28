using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Deliveries.Commands;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Application.Features.Orders.Notifications;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Orders.Commands;

internal sealed class UpdateOrderStatusCommandHandler(IAppDbContext db, ISender sender, IPublisher publisher)
    : IRequestHandler<UpdateOrderStatusCommand, Result>
{
    private static readonly Dictionary<OrderStatus, OrderStatus> ValidTransitions = new()
    {
        { OrderStatus.Placed, OrderStatus.Confirmed },
        { OrderStatus.Confirmed, OrderStatus.Preparing },
        { OrderStatus.Preparing, OrderStatus.ReadyForPickup },
        { OrderStatus.ReadyForPickup, OrderStatus.OutForDelivery },
        { OrderStatus.OutForDelivery, OrderStatus.Delivered },
    };

    public async Task<Result> Handle(UpdateOrderStatusCommand request, CancellationToken ct)
    {
        var order = await db.Orders
            .Include(o => o.Restaurant)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);

        if (order is null)
            return Result.Failure("ORDER_NOT_FOUND", "Order not found.");

        // Validate ownership — only the restaurant owner can update status
        if (order.Restaurant.OwnerId != request.UserId)
            return Result.Failure("UNAUTHORIZED", "You are not authorized to update this order.");

        // Validate transition
        if (!ValidTransitions.TryGetValue(order.Status, out var expectedNext) || expectedNext != request.NewStatus)
            return Result.Failure("INVALID_STATUS_TRANSITION",
                $"Cannot transition from {order.Status} to {request.NewStatus}.");

        order.Status = request.NewStatus;

        if (request.NewStatus == OrderStatus.Delivered)
            order.ActualDeliveryTime = DateTimeOffset.UtcNow;

        db.OrderStatusHistory.Add(new OrderStatusHistory
        {
            Id = Guid.CreateVersion7(),
            OrderId = order.Id,
            Status = request.NewStatus,
            Notes = request.Notes,
            ChangedBy = request.UserId,
            CreatedAt = DateTimeOffset.UtcNow,
        });

        await db.SaveChangesAsync(ct);

        await publisher.Publish(new OrderStatusChangedNotification(
            order.Id, order.UserId, request.NewStatus.ToString(), order.OrderNumber), ct);

        // Auto-assign delivery partner when order is confirmed (delivery orders only)
        if (request.NewStatus == OrderStatus.Confirmed && order.OrderType == OrderType.Delivery)
        {
            _ = sender.Send(new AssignDeliveryPartnerCommand(order.Id), ct);
        }

        return Result.Success();
    }
}
