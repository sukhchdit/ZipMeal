using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.ChatSupport.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.ChatSupport.Queries;

internal sealed class GetTicketMessagesQueryHandler(IAppDbContext db)
    : IRequestHandler<GetTicketMessagesQuery, Result<CursorPagedResult<SupportMessageDto>>>
{
    public async Task<Result<CursorPagedResult<SupportMessageDto>>> Handle(
        GetTicketMessagesQuery request, CancellationToken ct)
    {
        var ticketExists = await db.SupportTickets.AsNoTracking()
            .AnyAsync(t => t.Id == request.TicketId
                && (t.UserId == request.UserId || t.AssignedAgentId == request.UserId), ct);

        if (!ticketExists)
            return Result<CursorPagedResult<SupportMessageDto>>.Failure(
                "TICKET_NOT_FOUND", "Ticket not found or not authorized.");

        var query = db.SupportMessages.AsNoTracking()
            .Where(m => m.TicketId == request.TicketId)
            .OrderByDescending(m => m.CreatedAt);

        if (request.Cursor is not null
            && DateTimeOffset.TryParse(request.Cursor, out var cursorDate))
        {
            query = (IOrderedQueryable<Domain.Entities.SupportMessage>)
                query.Where(m => m.CreatedAt < cursorDate);
        }

        var pageSize = Math.Clamp(request.PageSize, 1, 50);

        var messages = await query
            .Take(pageSize + 1)
            .Select(m => new SupportMessageDto(
                m.Id,
                m.TicketId,
                m.SenderId,
                m.Sender.FullName,
                m.Content,
                (int)m.MessageType,
                m.IsRead,
                m.ReadAt,
                m.CreatedAt))
            .ToListAsync(ct);

        var hasMore = messages.Count > pageSize;
        if (hasMore) messages.RemoveAt(messages.Count - 1);

        var nextCursor = messages.Count > 0
            ? messages[^1].CreatedAt.ToString("O")
            : null;

        return Result<CursorPagedResult<SupportMessageDto>>.Success(
            new CursorPagedResult<SupportMessageDto>(
                messages, nextCursor, null, hasMore, pageSize));
    }
}
