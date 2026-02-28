using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Application.Features.DineIn.Notifications;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.DineIn.Commands;

internal sealed class UpdateDineInOrderStatusCommandHandler(IAppDbContext db, IPublisher publisher)
    : IRequestHandler<UpdateDineInOrderStatusCommand, Result>
{
    /// <summary>
    /// Valid dine-in order status transitions.
    /// Placed → Confirmed → Preparing → Ready → Served → Completed
    /// </summary>
    private static readonly Dictionary<DineInOrderStatus, DineInOrderStatus> ValidTransitions = new()
    {
        { DineInOrderStatus.Placed, DineInOrderStatus.Confirmed },
        { DineInOrderStatus.Confirmed, DineInOrderStatus.Preparing },
        { DineInOrderStatus.Preparing, DineInOrderStatus.Ready },
        { DineInOrderStatus.Ready, DineInOrderStatus.Served },
        { DineInOrderStatus.Served, DineInOrderStatus.Completed },
    };

    public async Task<Result> Handle(
        UpdateDineInOrderStatusCommand request, CancellationToken ct)
    {
        var order = await db.Orders
            .Include(o => o.Restaurant)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);

        if (order is null)
            return Result.Failure("ORDER_NOT_FOUND", "Order not found.");

        // Verify this is a dine-in order
        if (order.OrderType != OrderType.DineIn)
            return Result.Failure("NOT_DINE_IN_ORDER", "This order is not a dine-in order.");

        // Verify restaurant ownership
        if (order.Restaurant.OwnerId != request.OwnerId)
            return Result.Failure("UNAUTHORIZED", "You are not authorized to update this order.");

        // Cast current OrderStatus to DineInOrderStatus for transition validation
        var currentDineInStatus = (DineInOrderStatus)(short)order.Status;

        if (!ValidTransitions.TryGetValue(currentDineInStatus, out var expectedNext)
            || expectedNext != request.NewStatus)
        {
            return Result.Failure("INVALID_STATUS_TRANSITION",
                $"Cannot transition from {currentDineInStatus} to {request.NewStatus}.");
        }

        // Persist by casting DineInOrderStatus back to OrderStatus
        order.Status = (OrderStatus)(short)request.NewStatus;

        if (request.NewStatus == DineInOrderStatus.Served)
            order.ActualDeliveryTime = DateTimeOffset.UtcNow;

        db.OrderStatusHistory.Add(new OrderStatusHistory
        {
            Id = Guid.CreateVersion7(),
            OrderId = order.Id,
            Status = (OrderStatus)(short)request.NewStatus,
            Notes = request.Notes,
            ChangedBy = request.OwnerId,
            CreatedAt = DateTimeOffset.UtcNow,
        });

        await db.SaveChangesAsync(ct);

        if (order.DineInSessionId.HasValue)
        {
            await publisher.Publish(new DineInOrderStatusChangedNotification(
                order.DineInSessionId.Value, order.Id, request.NewStatus.ToString()), ct);
        }

        return Result.Success();
    }
}
