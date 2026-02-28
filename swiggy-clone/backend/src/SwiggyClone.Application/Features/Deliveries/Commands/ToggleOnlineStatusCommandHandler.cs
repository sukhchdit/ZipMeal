using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Deliveries.Commands;

internal sealed class ToggleOnlineStatusCommandHandler(IAppDbContext db)
    : IRequestHandler<ToggleOnlineStatusCommand, Result>
{
    public async Task<Result> Handle(ToggleOnlineStatusCommand request, CancellationToken ct)
    {
        var location = await db.DeliveryPartnerLocations
            .FirstOrDefaultAsync(l => l.PartnerId == request.PartnerId, ct);

        var now = DateTimeOffset.UtcNow;

        if (location is not null)
        {
            location.IsOnline = request.IsOnline;
            if (request.Latitude.HasValue) location.Latitude = request.Latitude.Value;
            if (request.Longitude.HasValue) location.Longitude = request.Longitude.Value;
            location.UpdatedAt = now;
        }
        else
        {
            db.DeliveryPartnerLocations.Add(new DeliveryPartnerLocation
            {
                Id = Guid.CreateVersion7(),
                PartnerId = request.PartnerId,
                Latitude = request.Latitude ?? 0,
                Longitude = request.Longitude ?? 0,
                IsOnline = request.IsOnline,
                UpdatedAt = now,
            });
        }

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
