using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Disputes.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Disputes.Queries.GetAllDisputes;

internal sealed class GetAllDisputesQueryHandler(IAppDbContext db)
    : IRequestHandler<GetAllDisputesQuery, Result<CursorPagedResult<DisputeSummaryDto>>>
{
    public async Task<Result<CursorPagedResult<DisputeSummaryDto>>> Handle(
        GetAllDisputesQuery request, CancellationToken ct)
    {
        var baseQuery = db.Disputes.AsNoTracking().AsQueryable();

        if (request.Status.HasValue)
            baseQuery = baseQuery.Where(d => d.Status == (DisputeStatus)request.Status.Value);

        if (request.IssueType.HasValue)
            baseQuery = baseQuery.Where(d => d.IssueType == (DisputeIssueType)request.IssueType.Value);

        if (request.AgentId.HasValue)
            baseQuery = baseQuery.Where(d => d.AssignedAgentId == request.AgentId.Value);

        var query = baseQuery.OrderByDescending(d => d.UpdatedAt);

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
                    .OrderByDescending(m => m.CreatedAt)
                    .Select(m => m.Content)
                    .FirstOrDefault(),
                0,
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
