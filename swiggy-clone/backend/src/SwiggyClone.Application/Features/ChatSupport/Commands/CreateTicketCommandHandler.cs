using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.ChatSupport.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.ChatSupport.Commands;

internal sealed class CreateTicketCommandHandler(IAppDbContext db)
    : IRequestHandler<CreateTicketCommand, Result<SupportTicketDto>>
{
    public async Task<Result<SupportTicketDto>> Handle(
        CreateTicketCommand request, CancellationToken ct)
    {
        // Validate order ownership if OrderId is provided
        if (request.OrderId.HasValue)
        {
            var orderExists = await db.Orders.AsNoTracking()
                .AnyAsync(o => o.Id == request.OrderId.Value && o.UserId == request.UserId, ct);

            if (!orderExists)
                return Result<SupportTicketDto>.Failure("ORDER_NOT_FOUND", "Order not found or does not belong to you.");
        }

        var ticket = new SupportTicket
        {
            Id = Guid.CreateVersion7(),
            UserId = request.UserId,
            Subject = request.Subject,
            Category = (SupportTicketCategory)request.Category,
            Status = SupportTicketStatus.Open,
            OrderId = request.OrderId,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
        db.SupportTickets.Add(ticket);

        string? lastMessage = null;

        // Add initial message if provided
        if (!string.IsNullOrWhiteSpace(request.InitialMessage))
        {
            var message = new SupportMessage
            {
                Id = Guid.CreateVersion7(),
                TicketId = ticket.Id,
                SenderId = request.UserId,
                Content = request.InitialMessage,
                MessageType = SupportMessageType.Text,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
            };
            db.SupportMessages.Add(message);
            lastMessage = request.InitialMessage;
        }

        // Add system message
        var systemMessage = new SupportMessage
        {
            Id = Guid.CreateVersion7(),
            TicketId = ticket.Id,
            SenderId = request.UserId,
            Content = "Ticket created. A support agent will be with you shortly.",
            MessageType = SupportMessageType.System,
            IsRead = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
        db.SupportMessages.Add(systemMessage);

        await db.SaveChangesAsync(ct);

        var user = await db.Users.AsNoTracking()
            .FirstAsync(u => u.Id == request.UserId, ct);

        return Result<SupportTicketDto>.Success(new SupportTicketDto(
            ticket.Id,
            ticket.UserId,
            user.FullName,
            null,
            null,
            ticket.Subject,
            (int)ticket.Category,
            (int)ticket.Status,
            ticket.OrderId,
            lastMessage,
            null,
            ticket.CreatedAt,
            ticket.UpdatedAt));
    }
}
