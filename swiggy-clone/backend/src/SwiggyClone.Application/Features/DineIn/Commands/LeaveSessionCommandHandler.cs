using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Application.Features.DineIn.Notifications;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.DineIn.Commands;

internal sealed class LeaveSessionCommandHandler(IAppDbContext db, IPublisher publisher)
    : IRequestHandler<LeaveSessionCommand, Result>
{
    public async Task<Result> Handle(LeaveSessionCommand request, CancellationToken ct)
    {
        // 1. Find session
        var session = await db.DineInSessions
            .FirstOrDefaultAsync(s => s.Id == request.SessionId
                && s.Status == DineInSessionStatus.Active, ct);

        if (session is null)
            return Result.Failure("SESSION_NOT_FOUND", "No active dine-in session found.");

        // 2. Find member
        var member = await db.DineInSessionMembers
            .FirstOrDefaultAsync(m => m.SessionId == request.SessionId
                && m.UserId == request.UserId, ct);

        if (member is null)
            return Result.Failure("NOT_SESSION_MEMBER", "You are not a member of this session.");

        // 3. Host cannot leave
        if (member.Role == 1)
            return Result.Failure("HOST_CANNOT_LEAVE",
                "The session host cannot leave. Please end the session instead.");

        // 4. Remove member
        db.DineInSessionMembers.Remove(member);
        session.GuestCount = Math.Max(1, session.GuestCount - 1);
        session.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);

        await publisher.Publish(new DineInMemberLeftNotification(
            session.Id, request.UserId), ct);

        return Result.Success();
    }
}
