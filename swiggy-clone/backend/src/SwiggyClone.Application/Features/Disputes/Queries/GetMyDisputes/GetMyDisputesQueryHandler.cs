using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Disputes.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Disputes.Queries.GetMyDisputes;

internal sealed class GetMyDisputesQueryHandler(IAppDbContext db)
    : IRequestHandler<GetMyDisputesQuery, Result<CursorPagedResult<DisputeSummaryDto>>>
{
    public async Task<Result<CursorPagedResult<DisputeSummaryDto>>> Handle(
        GetMyDisputesQuery request, CancellationToken ct)
    {
        var query = db.Disputes.AsNoTracking()
            .Where(d => d.UserId == request.UserId)
            .OrderByDescending(d => d.UpdatedAt);

        if (request.Cursor is not null
            && DateTimeOffset.TryParse(request.Cursor, out var cursorDate))
        {
            query = (IOrderedQueryable<Domain.Entities.Dispute>)
                query.Where(d => d.UpdatedAt < cursorDate);
        }

        var pageSize = Math.Clamp(request.PageSize, 1, 50);

        var disputes = await query
            .Take(pageSize + 1)
            .Select(d => new DisputeSummaryDto(
                d.Id,
                d.DisputeNumber,
                d.Order.OrderNumber,
                (int)d.IssueType,
                (int)d.Status,
                d.Messages
                    .Where(m => !m.IsSystemMessage)
                    .OrderByDescending(m => m.CreatedAt)
                    .Select(m => m.Content)
                    .FirstOrDefault(),
                d.Messages.Count(m => !m.IsRead && m.SenderId != request.UserId),
                d.CreatedAt,
                d.UpdatedAt))
            .ToListAsync(ct);

        var hasMore = disputes.Count > pageSize;
        if (hasMore) disputes.RemoveAt(disputes.Count - 1);

        var nextCursor = disputes.Count > 0
            ? disputes[^1].UpdatedAt.ToString("O")
            : null;

        return Result<CursorPagedResult<DisputeSummaryDto>>.Success(
            new CursorPagedResult<DisputeSummaryDto>(
                disputes, nextCursor, null, hasMore, pageSize));
    }
}
