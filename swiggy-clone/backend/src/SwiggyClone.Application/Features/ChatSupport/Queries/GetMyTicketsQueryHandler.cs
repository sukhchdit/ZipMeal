using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.ChatSupport.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.ChatSupport.Queries;

internal sealed class GetMyTicketsQueryHandler(IAppDbContext db)
    : IRequestHandler<GetMyTicketsQuery, Result<CursorPagedResult<SupportTicketSummaryDto>>>
{
    public async Task<Result<CursorPagedResult<SupportTicketSummaryDto>>> Handle(
        GetMyTicketsQuery request, CancellationToken ct)
    {
        var query = db.SupportTickets.AsNoTracking()
            .Where(t => t.UserId == request.UserId)
            .OrderByDescending(t => t.UpdatedAt);

        if (request.Cursor is not null
            && DateTimeOffset.TryParse(request.Cursor, out var cursorDate))
        {
            query = (IOrderedQueryable<Domain.Entities.SupportTicket>)
                query.Where(t => t.UpdatedAt < cursorDate);
        }

        var pageSize = Math.Clamp(request.PageSize, 1, 50);

        var tickets = await query
            .Take(pageSize + 1)
            .Select(t => new SupportTicketSummaryDto(
                t.Id,
                t.Subject,
                (int)t.Category,
                (int)t.Status,
                t.Messages
                    .Where(m => m.MessageType != Domain.Enums.SupportMessageType.System)
                    .OrderByDescending(m => m.CreatedAt)
                    .Select(m => m.Content)
                    .FirstOrDefault(),
                t.Messages.Count(m => !m.IsRead && m.SenderId != request.UserId),
                t.CreatedAt,
                t.UpdatedAt))
            .ToListAsync(ct);

        var hasMore = tickets.Count > pageSize;
        if (hasMore) tickets.RemoveAt(tickets.Count - 1);

        var nextCursor = tickets.Count > 0
            ? tickets[^1].UpdatedAt.ToString("O")
            : null;

        return Result<CursorPagedResult<SupportTicketSummaryDto>>.Success(
            new CursorPagedResult<SupportTicketSummaryDto>(
                tickets, nextCursor, null, hasMore, pageSize));
    }
}
