using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Deliveries.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Application.Features.Deliveries.Notifications;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Deliveries.Commands;

internal sealed class AssignDeliveryPartnerCommandHandler(IAppDbContext db, IPublisher publisher)
    : IRequestHandler<AssignDeliveryPartnerCommand, Result<DeliveryAssignmentDto>>
{
    private const int BaseEarningsPaise = 3000;  // ₹30 base
    private const int PerKmEarningsPaise = 500;  // ₹5 per km

    public async Task<Result<DeliveryAssignmentDto>> Handle(
        AssignDeliveryPartnerCommand request, CancellationToken ct)
    {
        var order = await db.Orders
            .Include(o => o.Restaurant)
            .Include(o => o.DeliveryAddress)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);

        if (order is null)
            return Result<DeliveryAssignmentDto>.Failure("ORDER_NOT_FOUND", "Order not found.");

        if (order.OrderType != OrderType.Delivery)
            return Result<DeliveryAssignmentDto>.Failure(
                "NOT_DELIVERY_ORDER", "Only delivery orders can be assigned.");

        // Check if already assigned
        var existing = await db.DeliveryAssignments
            .AnyAsync(a => a.OrderId == request.OrderId && a.Status != DeliveryStatus.Cancelled, ct);

        if (existing)
            return Result<DeliveryAssignmentDto>.Failure(
                "ALREADY_ASSIGNED", "A delivery partner is already assigned to this order.");

        // Find active delivery assignment IDs to exclude busy partners
        var busyPartnerIds = await db.DeliveryAssignments
            .Where(a => a.Status != DeliveryStatus.Delivered && a.Status != DeliveryStatus.Cancelled)
            .Select(a => a.PartnerId)
            .Distinct()
            .ToListAsync(ct);

        // Find nearest available online partner
        var availablePartner = await db.DeliveryPartnerLocations
            .Where(l => l.IsOnline && !busyPartnerIds.Contains(l.PartnerId))
            .OrderBy(l => l.UpdatedAt) // Simple: pick most recently updated (nearest proxy)
            .FirstOrDefaultAsync(ct);

        if (availablePartner is null)
            return Result<DeliveryAssignmentDto>.Failure(
                "NO_PARTNER_AVAILABLE", "No delivery partners are currently available.");

        var now = DateTimeOffset.UtcNow;
        var estimatedDistance = 5.0m; // Default 5km estimate
        var earnings = BaseEarningsPaise + (int)(estimatedDistance * PerKmEarningsPaise);

        var assignment = new DeliveryAssignment
        {
            Id = Guid.CreateVersion7(),
            OrderId = request.OrderId,
            PartnerId = availablePartner.PartnerId,
            Status = DeliveryStatus.Assigned,
            AssignedAt = now,
            CurrentLatitude = availablePartner.Latitude,
            CurrentLongitude = availablePartner.Longitude,
            DistanceKm = estimatedDistance,
            Earnings = earnings,
            CreatedAt = now,
            UpdatedAt = now,
        };

        db.DeliveryAssignments.Add(assignment);
        await db.SaveChangesAsync(ct);

        await publisher.Publish(new DeliveryAssignedNotification(
            assignment.Id, assignment.OrderId, assignment.PartnerId, order.UserId), ct);

        return Result<DeliveryAssignmentDto>.Success(new DeliveryAssignmentDto(
            assignment.Id, assignment.OrderId, order.OrderNumber,
            order.Restaurant.Name, null,
            order.DeliveryAddress?.AddressLine1,
            (int)assignment.Status, assignment.AssignedAt,
            assignment.AcceptedAt, assignment.PickedUpAt,
            assignment.DeliveredAt, assignment.DistanceKm, assignment.Earnings));
    }
}
