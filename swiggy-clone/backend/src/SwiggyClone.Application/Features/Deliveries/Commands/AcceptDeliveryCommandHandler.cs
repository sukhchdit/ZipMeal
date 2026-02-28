using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Deliveries.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Application.Features.Deliveries.Notifications;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Deliveries.Commands;

internal sealed class AcceptDeliveryCommandHandler(IAppDbContext db, IPublisher publisher)
    : IRequestHandler<AcceptDeliveryCommand, Result<DeliveryAssignmentDto>>
{
    public async Task<Result<DeliveryAssignmentDto>> Handle(
        AcceptDeliveryCommand request, CancellationToken ct)
    {
        var assignment = await db.DeliveryAssignments
            .Include(a => a.Order)
                .ThenInclude(o => o.Restaurant)
            .Include(a => a.Order)
                .ThenInclude(o => o.DeliveryAddress)
            .FirstOrDefaultAsync(a => a.Id == request.AssignmentId, ct);

        if (assignment is null)
            return Result<DeliveryAssignmentDto>.Failure(
                "ASSIGNMENT_NOT_FOUND", "Delivery assignment not found.");

        if (assignment.PartnerId != request.PartnerId)
            return Result<DeliveryAssignmentDto>.Failure(
                "UNAUTHORIZED", "This assignment is not assigned to you.");

        if (assignment.Status != DeliveryStatus.Assigned)
            return Result<DeliveryAssignmentDto>.Failure(
                "INVALID_STATUS", "Assignment has already been accepted or completed.");

        var now = DateTimeOffset.UtcNow;
        assignment.Status = DeliveryStatus.Accepted;
        assignment.AcceptedAt = now;
        assignment.UpdatedAt = now;

        // Link partner to order
        assignment.Order.DeliveryPartnerId = request.PartnerId;

        db.OrderStatusHistory.Add(new OrderStatusHistory
        {
            Id = Guid.CreateVersion7(),
            OrderId = assignment.OrderId,
            Status = assignment.Order.Status,
            Notes = "Delivery partner accepted the assignment.",
            ChangedBy = request.PartnerId,
            CreatedAt = now,
        });

        await db.SaveChangesAsync(ct);

        await publisher.Publish(new DeliveryStatusChangedNotification(
            assignment.Id, assignment.OrderId, assignment.Order.UserId, "Accepted"), ct);

        return Result<DeliveryAssignmentDto>.Success(MapToDto(assignment));
    }

    private static DeliveryAssignmentDto MapToDto(DeliveryAssignment a) => new(
        a.Id, a.OrderId, a.Order.OrderNumber, a.Order.Restaurant.Name,
        null, // RestaurantAddress — simplified
        a.Order.DeliveryAddress?.AddressLine1,
        (int)a.Status, a.AssignedAt, a.AcceptedAt, a.PickedUpAt,
        a.DeliveredAt, a.DistanceKm, a.Earnings);
}
