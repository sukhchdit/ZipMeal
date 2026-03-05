using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Disputes.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Disputes.Queries.GetDisputeMessages;

internal sealed class GetDisputeMessagesQueryHandler(IAppDbContext db)
    : IRequestHandler<GetDisputeMessagesQuery, Result<CursorPagedResult<DisputeMessageDto>>>
{
    public async Task<Result<CursorPagedResult<DisputeMessageDto>>> Handle(
        GetDisputeMessagesQuery request, CancellationToken ct)
    {
        // Validate access
        var disputeExists = await db.Disputes.AsNoTracking()
            .AnyAsync(d => d.Id == request.DisputeId
                && (d.UserId == request.UserId || d.AssignedAgentId == request.UserId), ct);

        if (!disputeExists)
            return Result<CursorPagedResult<DisputeMessageDto>>.Failure(
                "DISPUTE_NOT_FOUND", "Dispute not found.");

        var query = db.DisputeMessages.AsNoTracking()
            .Where(m => m.DisputeId == request.DisputeId)
            .OrderByDescending(m => m.CreatedAt);

        if (request.Cursor is not null
            && DateTimeOffset.TryParse(request.Cursor, out var cursorDate))
        {
            query = (IOrderedQueryable<Domain.Entities.DisputeMessage>)
                query.Where(m => m.CreatedAt < cursorDate);
        }

        var pageSize = Math.Clamp(request.PageSize, 1, 50);

        var messages = await query
            .Take(pageSize + 1)
            .Select(m => new DisputeMessageDto(
                m.Id,
                m.DisputeId,
                m.SenderId,
                m.Sender.FullName,
                m.Content,
                m.IsSystemMessage,
                m.IsRead,
                m.ReadAt,
                m.CreatedAt))
            .ToListAsync(ct);

        var hasMore = messages.Count > pageSize;
        if (hasMore) messages.RemoveAt(messages.Count - 1);

        var nextCursor = messages.Count > 0
            ? messages[^1].CreatedAt.ToString("O")
            : null;

        return Result<CursorPagedResult<DisputeMessageDto>>.Success(
            new CursorPagedResult<DisputeMessageDto>(
                messages, nextCursor, null, hasMore, pageSize));
    }
}
