using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.ChatSupport.Commands;

internal sealed class MarkMessagesReadCommandHandler(IAppDbContext db)
    : IRequestHandler<MarkMessagesReadCommand, Result>
{
    public async Task<Result> Handle(MarkMessagesReadCommand request, CancellationToken ct)
    {
        var ticketExists = await db.SupportTickets.AsNoTracking()
            .AnyAsync(t => t.Id == request.TicketId
                && (t.UserId == request.UserId || t.AssignedAgentId == request.UserId), ct);

        if (!ticketExists)
            return Result.Failure("TICKET_NOT_FOUND", "Ticket not found or not authorized.");

        var now = DateTimeOffset.UtcNow;

        // Mark all unread messages from the other party as read
        var unreadMessages = await db.SupportMessages
            .Where(m => m.TicketId == request.TicketId
                && m.SenderId != request.UserId
                && !m.IsRead)
            .ToListAsync(ct);

        foreach (var msg in unreadMessages)
        {
            msg.IsRead = true;
            msg.ReadAt = now;
        }

        await db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
