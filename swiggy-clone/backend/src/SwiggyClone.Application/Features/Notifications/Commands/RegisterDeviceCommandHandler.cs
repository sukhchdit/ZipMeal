using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Notifications.Commands;

internal sealed class RegisterDeviceCommandHandler(IAppDbContext db)
    : IRequestHandler<RegisterDeviceCommand, Result>
{
    public async Task<Result> Handle(RegisterDeviceCommand request, CancellationToken ct)
    {
        var existing = await db.UserDevices
            .FirstOrDefaultAsync(
                d => d.UserId == request.UserId && d.DeviceToken == request.DeviceToken, ct);

        if (existing is not null)
        {
            existing.IsActive = true;
            existing.Platform = (DevicePlatform)request.Platform;
            existing.UpdatedAt = DateTimeOffset.UtcNow;
        }
        else
        {
            var now = DateTimeOffset.UtcNow;
            db.UserDevices.Add(new UserDevice
            {
                Id = Guid.CreateVersion7(),
                UserId = request.UserId,
                DeviceToken = request.DeviceToken,
                Platform = (DevicePlatform)request.Platform,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now,
            });
        }

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
