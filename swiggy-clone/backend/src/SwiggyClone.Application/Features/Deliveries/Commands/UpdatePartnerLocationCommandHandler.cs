using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Application.Features.Deliveries.Notifications;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Deliveries.Commands;

internal sealed class UpdatePartnerLocationCommandHandler(IAppDbContext db, IPublisher publisher)
    : IRequestHandler<UpdatePartnerLocationCommand, Result>
{
    public async Task<Result> Handle(UpdatePartnerLocationCommand request, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;

        var location = await db.DeliveryPartnerLocations
            .FirstOrDefaultAsync(l => l.PartnerId == request.PartnerId, ct);

        if (location is not null)
        {
            location.Latitude = request.Latitude;
            location.Longitude = request.Longitude;
            location.Heading = request.Heading;
            location.Speed = request.Speed;
            location.UpdatedAt = now;
        }
        else
        {
            db.DeliveryPartnerLocations.Add(new DeliveryPartnerLocation
            {
                Id = Guid.CreateVersion7(),
                PartnerId = request.PartnerId,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Heading = request.Heading,
                Speed = request.Speed,
                IsOnline = true,
                UpdatedAt = now,
            });
        }

        // Also update active delivery assignment location
        var activeAssignment = await db.DeliveryAssignments
            .FirstOrDefaultAsync(a =>
                a.PartnerId == request.PartnerId &&
                a.Status != DeliveryStatus.Delivered &&
                a.Status != DeliveryStatus.Cancelled, ct);

        if (activeAssignment is not null)
        {
            activeAssignment.CurrentLatitude = request.Latitude;
            activeAssignment.CurrentLongitude = request.Longitude;
            activeAssignment.UpdatedAt = now;
        }

        await db.SaveChangesAsync(ct);

        if (activeAssignment is not null)
        {
            await publisher.Publish(new DeliveryLocationUpdatedNotification(
                activeAssignment.OrderId, request.PartnerId,
                (double)request.Latitude, (double)request.Longitude, request.Heading, request.Speed), ct);
        }

        return Result.Success();
    }
}
