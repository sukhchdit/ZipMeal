using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Notifications.Commands;

internal sealed class MarkNotificationAsReadCommandHandler(IAppDbContext db)
    : IRequestHandler<MarkNotificationAsReadCommand, Result>
{
    public async Task<Result> Handle(
        MarkNotificationAsReadCommand request,
        CancellationToken ct)
    {
        var notification = await db.Notifications
            .FirstOrDefaultAsync(
                n => n.Id == request.NotificationId && n.UserId == request.UserId, ct);

        if (notification is null)
            return Result.Failure("NOTIFICATION_NOT_FOUND", "Notification not found.");

        if (notification.IsRead)
            return Result.Success();

        notification.IsRead = true;
        notification.ReadAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
