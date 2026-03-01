using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.ChatSupport.Commands;

internal sealed class CloseTicketCommandHandler(IAppDbContext db, IRealtimeNotifier notifier)
    : IRequestHandler<CloseTicketCommand, Result>
{
    public async Task<Result> Handle(CloseTicketCommand request, CancellationToken ct)
    {
        var ticket = await db.SupportTickets
            .FirstOrDefaultAsync(t => t.Id == request.TicketId, ct);

        if (ticket is null)
            return Result.Failure("TICKET_NOT_FOUND", "Ticket not found.");

        if (ticket.Status == SupportTicketStatus.Closed)
            return Result.Failure("ALREADY_CLOSED", "Ticket is already closed.");

        ticket.Status = SupportTicketStatus.Closed;
        ticket.ClosedAt = DateTimeOffset.UtcNow;
        ticket.UpdatedAt = DateTimeOffset.UtcNow;

        var systemMessage = new SupportMessage
        {
            Id = Guid.CreateVersion7(),
            TicketId = ticket.Id,
            SenderId = request.UserId,
            Content = "This ticket has been closed.",
            MessageType = SupportMessageType.System,
            IsRead = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
        db.SupportMessages.Add(systemMessage);

        await db.SaveChangesAsync(ct);

        await notifier.NotifyChatMessageAsync(
            ticket.Id,
            ticket.UserId,
            new ChatSupport.DTOs.SupportMessageDto(
                systemMessage.Id, systemMessage.TicketId, systemMessage.SenderId,
                "System", systemMessage.Content, (int)systemMessage.MessageType,
                true, null, systemMessage.CreatedAt),
            ct);

        return Result.Success();
    }
}
