using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.ChatSupport.Commands;

internal sealed class AssignTicketCommandHandler(IAppDbContext db)
    : IRequestHandler<AssignTicketCommand, Result>
{
    public async Task<Result> Handle(AssignTicketCommand request, CancellationToken ct)
    {
        var ticket = await db.SupportTickets
            .FirstOrDefaultAsync(t => t.Id == request.TicketId, ct);

        if (ticket is null)
            return Result.Failure("TICKET_NOT_FOUND", "Ticket not found.");

        var agentExists = await db.Users.AsNoTracking()
            .AnyAsync(u => u.Id == request.AgentId, ct);

        if (!agentExists)
            return Result.Failure("AGENT_NOT_FOUND", "Agent not found.");

        ticket.AssignedAgentId = request.AgentId;
        ticket.UpdatedAt = DateTimeOffset.UtcNow;

        if (ticket.Status == SupportTicketStatus.Open)
        {
            ticket.Status = SupportTicketStatus.InProgress;
        }

        var agent = await db.Users.AsNoTracking()
            .FirstAsync(u => u.Id == request.AgentId, ct);

        var systemMessage = new SupportMessage
        {
            Id = Guid.CreateVersion7(),
            TicketId = ticket.Id,
            SenderId = request.AgentId,
            Content = $"{agent.FullName} has been assigned to this ticket.",
            MessageType = SupportMessageType.System,
            IsRead = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
        db.SupportMessages.Add(systemMessage);

        await db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
