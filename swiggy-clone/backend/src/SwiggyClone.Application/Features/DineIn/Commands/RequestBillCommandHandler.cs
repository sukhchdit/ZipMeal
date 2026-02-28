using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Application.Features.DineIn.Notifications;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.DineIn.Commands;

internal sealed class RequestBillCommandHandler(IAppDbContext db, IPublisher publisher)
    : IRequestHandler<RequestBillCommand, Result>
{
    public async Task<Result> Handle(RequestBillCommand request, CancellationToken ct)
    {
        // 1. Find session
        var session = await db.DineInSessions
            .FirstOrDefaultAsync(s => s.Id == request.SessionId
                && s.Status == DineInSessionStatus.Active, ct);

        if (session is null)
            return Result.Failure("SESSION_NOT_FOUND", "No active dine-in session found.");

        // 2. Validate host
        var isHost = await db.DineInSessionMembers
            .AnyAsync(m => m.SessionId == request.SessionId
                && m.UserId == request.UserId
                && m.Role == 1, ct);

        if (!isHost)
            return Result.Failure("NOT_HOST", "Only the session host can request the bill.");

        // 3. Update status
        session.Status = DineInSessionStatus.BillRequested;
        session.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);

        await publisher.Publish(new DineInBillRequestedNotification(
            session.Id, session.RestaurantId), ct);

        return Result.Success();
    }
}
