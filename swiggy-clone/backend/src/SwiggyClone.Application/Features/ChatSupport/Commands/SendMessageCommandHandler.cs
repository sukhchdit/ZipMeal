using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.ChatSupport.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.ChatSupport.Commands;

internal sealed class SendMessageCommandHandler(
    IAppDbContext db,
    IRealtimeNotifier notifier,
    INotificationService pushService)
    : IRequestHandler<SendMessageCommand, Result<SupportMessageDto>>
{
    public async Task<Result<SupportMessageDto>> Handle(
        SendMessageCommand request, CancellationToken ct)
    {
        var ticket = await db.SupportTickets.AsNoTracking()
            .Include(t => t.User)
            .Include(t => t.AssignedAgent)
            .FirstOrDefaultAsync(t => t.Id == request.TicketId, ct);

        if (ticket is null)
            return Result<SupportMessageDto>.Failure("TICKET_NOT_FOUND", "Ticket not found.");

        var isCustomer = ticket.UserId == request.UserId;
        var isAgent = ticket.AssignedAgentId == request.UserId;
        if (!isCustomer && !isAgent)
            return Result<SupportMessageDto>.Failure("UNAUTHORIZED", "Not authorized for this ticket.");

        if (ticket.Status == SupportTicketStatus.Closed)
            return Result<SupportMessageDto>.Failure("TICKET_CLOSED", "This ticket has been closed.");

        var message = new SupportMessage
        {
            Id = Guid.CreateVersion7(),
            TicketId = request.TicketId,
            SenderId = request.UserId,
            Content = request.Content,
            MessageType = (SupportMessageType)request.MessageType,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
        db.SupportMessages.Add(message);

        // Update ticket status if customer replying to WaitingOnCustomer
        if (isCustomer && ticket.Status == SupportTicketStatus.WaitingOnCustomer)
        {
            var tracked = await db.SupportTickets.FindAsync([request.TicketId], ct);
            tracked!.Status = SupportTicketStatus.InProgress;
            tracked.UpdatedAt = DateTimeOffset.UtcNow;
        }

        await db.SaveChangesAsync(ct);

        var senderName = isCustomer
            ? ticket.User.FullName
            : ticket.AssignedAgent?.FullName ?? "Support";

        var dto = new SupportMessageDto(
            message.Id,
            message.TicketId,
            message.SenderId,
            senderName,
            message.Content,
            (int)message.MessageType,
            false,
            null,
            message.CreatedAt);

        var recipientId = isCustomer ? ticket.AssignedAgentId : ticket.UserId;
        if (recipientId.HasValue)
        {
            await notifier.NotifyChatMessageAsync(request.TicketId, recipientId.Value, dto, ct);

            var truncatedContent = request.Content.Length > 100
                ? request.Content[..100] + "..."
                : request.Content;
            _ = pushService.SendPushAsync(
                recipientId.Value,
                $"New message from {senderName}",
                truncatedContent,
                null,
                ct);
        }

        return Result<SupportMessageDto>.Success(dto);
    }
}
