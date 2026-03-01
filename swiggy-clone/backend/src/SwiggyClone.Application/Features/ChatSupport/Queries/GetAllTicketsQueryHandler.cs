using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.ChatSupport.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.ChatSupport.Queries;

internal sealed class GetAllTicketsQueryHandler(IAppDbContext db)
    : IRequestHandler<GetAllTicketsQuery, Result<CursorPagedResult<SupportTicketDto>>>
{
    public async Task<Result<CursorPagedResult<SupportTicketDto>>> Handle(
        GetAllTicketsQuery request, CancellationToken ct)
    {
        var baseQuery = db.SupportTickets.AsNoTracking()
            .Include(t => t.User)
            .Include(t => t.AssignedAgent)
            .AsQueryable();

        if (request.Status.HasValue)
            baseQuery = baseQuery.Where(t => t.Status == (SupportTicketStatus)request.Status.Value);

        if (request.Category.HasValue)
            baseQuery = baseQuery.Where(t => t.Category == (SupportTicketCategory)request.Category.Value);

        if (request.AgentId.HasValue)
            baseQuery = baseQuery.Where(t => t.AssignedAgentId == request.AgentId.Value);

        var query = baseQuery.OrderByDescending(t => t.UpdatedAt);

        if (request.Cursor is not null
            && DateTimeOffset.TryParse(request.Cursor, out var cursorDate))
        {
            query = (IOrderedQueryable<Domain.Entities.SupportTicket>)
                query.Where(t => t.UpdatedAt < cursorDate);
        }

        var pageSize = Math.Clamp(request.PageSize, 1, 50);

        var tickets = await query
            .Take(pageSize + 1)
            .Select(t => new SupportTicketDto(
                t.Id,
                t.UserId,
                t.User.FullName,
                t.AssignedAgentId,
                t.AssignedAgent != null ? t.AssignedAgent.FullName : null,
                t.Subject,
                (int)t.Category,
                (int)t.Status,
                t.OrderId,
                t.Messages
                    .OrderByDescending(m => m.CreatedAt)
                    .Select(m => m.Content)
                    .FirstOrDefault(),
                t.ClosedAt,
                t.CreatedAt,
                t.UpdatedAt))
            .ToListAsync(ct);

        var hasMore = tickets.Count > pageSize;
        if (hasMore) tickets.RemoveAt(tickets.Count - 1);

        var nextCursor = tickets.Count > 0
            ? tickets[^1].UpdatedAt.ToString("O")
            : null;

        return Result<CursorPagedResult<SupportTicketDto>>.Success(
            new CursorPagedResult<SupportTicketDto>(
                tickets, nextCursor, null, hasMore, pageSize));
    }
}
