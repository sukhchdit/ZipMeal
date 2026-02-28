using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Application.Features.Deliveries.Notifications;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Deliveries.Commands;

internal sealed class UpdateDeliveryStatusCommandHandler(IAppDbContext db, IPublisher publisher)
    : IRequestHandler<UpdateDeliveryStatusCommand, Result>
{
    private static readonly Dictionary<DeliveryStatus, DeliveryStatus> ValidTransitions = new()
    {
        { DeliveryStatus.Accepted, DeliveryStatus.PickedUp },
        { DeliveryStatus.PickedUp, DeliveryStatus.EnRoute },
        { DeliveryStatus.EnRoute, DeliveryStatus.Delivered },
    };

    public async Task<Result> Handle(UpdateDeliveryStatusCommand request, CancellationToken ct)
    {
        var assignment = await db.DeliveryAssignments
            .Include(a => a.Order)
            .FirstOrDefaultAsync(a => a.Id == request.AssignmentId, ct);

        if (assignment is null)
            return Result.Failure("ASSIGNMENT_NOT_FOUND", "Delivery assignment not found.");

        if (assignment.PartnerId != request.PartnerId)
            return Result.Failure("UNAUTHORIZED", "This assignment is not assigned to you.");

        var newStatus = (DeliveryStatus)request.NewStatus;

        if (!ValidTransitions.TryGetValue(assignment.Status, out var expected) || expected != newStatus)
            return Result.Failure("INVALID_STATUS_TRANSITION",
                $"Cannot transition from {assignment.Status} to {newStatus}.");

        var now = DateTimeOffset.UtcNow;
        assignment.Status = newStatus;
        assignment.UpdatedAt = now;

        // Update timestamp fields
        switch (newStatus)
        {
            case DeliveryStatus.PickedUp:
                assignment.PickedUpAt = now;
                break;
            case DeliveryStatus.EnRoute:
                assignment.Order.Status = OrderStatus.OutForDelivery;
                break;
            case DeliveryStatus.Delivered:
                assignment.DeliveredAt = now;
                assignment.Order.Status = OrderStatus.Delivered;
                assignment.Order.ActualDeliveryTime = now;
                break;
        }

        db.OrderStatusHistory.Add(new OrderStatusHistory
        {
            Id = Guid.CreateVersion7(),
            OrderId = assignment.OrderId,
            Status = assignment.Order.Status,
            Notes = $"Delivery status updated to {newStatus}.",
            ChangedBy = request.PartnerId,
            CreatedAt = now,
        });

        await db.SaveChangesAsync(ct);

        await publisher.Publish(new DeliveryStatusChangedNotification(
            assignment.Id, assignment.OrderId, assignment.Order.UserId, newStatus.ToString()), ct);

        return Result.Success();
    }
}
