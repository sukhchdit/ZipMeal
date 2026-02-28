using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Notifications.Commands;

internal sealed class UnregisterDeviceCommandHandler(IAppDbContext db)
    : IRequestHandler<UnregisterDeviceCommand, Result>
{
    public async Task<Result> Handle(UnregisterDeviceCommand request, CancellationToken ct)
    {
        var device = await db.UserDevices
            .FirstOrDefaultAsync(
                d => d.UserId == request.UserId && d.DeviceToken == request.DeviceToken, ct);

        if (device is null)
            return Result.Failure("DEVICE_NOT_FOUND", "Device not found.");

        device.IsActive = false;
        device.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
