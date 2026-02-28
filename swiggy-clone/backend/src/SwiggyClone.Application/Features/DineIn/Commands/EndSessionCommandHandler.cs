using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Application.Features.DineIn.Notifications;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.DineIn.Commands;

internal sealed class EndSessionCommandHandler(IAppDbContext db, IPublisher publisher)
    : IRequestHandler<EndSessionCommand, Result>
{
    public async Task<Result> Handle(EndSessionCommand request, CancellationToken ct)
    {
        // 1. Find session
        var session = await db.DineInSessions
            .FirstOrDefaultAsync(s => s.Id == request.SessionId, ct);

        if (session is null)
            return Result.Failure("SESSION_NOT_FOUND", "Dine-in session not found.");

        if (session.Status != DineInSessionStatus.BillRequested
            && session.Status != DineInSessionStatus.PaymentPending)
            return Result.Failure("INVALID_SESSION_STATUS",
                "Session must be in BillRequested or PaymentPending status to end.");

        // 2. Validate host
        var isHost = await db.DineInSessionMembers
            .AnyAsync(m => m.SessionId == request.SessionId
                && m.UserId == request.UserId
                && m.Role == 1, ct);

        if (!isHost)
            return Result.Failure("NOT_HOST", "Only the session host can end the session.");

        // 3. End session
        var now = DateTimeOffset.UtcNow;
        session.Status = DineInSessionStatus.Completed;
        session.EndedAt = now;
        session.UpdatedAt = now;

        // 4. Free the table
        var table = await db.RestaurantTables
            .FirstAsync(t => t.Id == session.TableId, ct);

        table.Status = TableStatus.Available;
        table.UpdatedAt = now;

        await db.SaveChangesAsync(ct);

        await publisher.Publish(new DineInSessionEndedNotification(
            session.Id, session.RestaurantId, table.TableNumber), ct);

        return Result.Success();
    }
}
