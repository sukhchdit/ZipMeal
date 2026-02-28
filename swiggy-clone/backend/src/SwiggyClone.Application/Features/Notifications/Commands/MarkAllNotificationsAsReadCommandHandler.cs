using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Notifications.Commands;

internal sealed class MarkAllNotificationsAsReadCommandHandler(IAppDbContext db)
    : IRequestHandler<MarkAllNotificationsAsReadCommand, Result>
{
    public async Task<Result> Handle(
        MarkAllNotificationsAsReadCommand request,
        CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;

        await db.Notifications
            .Where(n => n.UserId == request.UserId && !n.IsRead)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(n => n.IsRead, true)
                .SetProperty(n => n.ReadAt, now), ct);

        return Result.Success();
    }
}
