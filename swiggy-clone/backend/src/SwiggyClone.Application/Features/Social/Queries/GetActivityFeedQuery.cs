using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Social.Dtos;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Social.Queries;

public sealed record GetActivityFeedQuery(
    Guid UserId,
    DateTimeOffset? Cursor,
    int PageSize = 20) : IRequest<Result<ActivityFeedResponse>>;

internal sealed class GetActivityFeedQueryHandler(IAppDbContext db)
    : IRequestHandler<GetActivityFeedQuery, Result<ActivityFeedResponse>>
{
    public async Task<Result<ActivityFeedResponse>> Handle(
        GetActivityFeedQuery request, CancellationToken ct)
    {
        var followingIds = await db.UserFollows
            .AsNoTracking()
            .Where(f => f.FollowerId == request.UserId)
            .Select(f => f.FollowingId)
            .ToListAsync(ct);

        if (followingIds.Count == 0)
        {
            return Result<ActivityFeedResponse>.Success(
                new ActivityFeedResponse([], null, false));
        }

        var query = db.ActivityFeedItems
            .AsNoTracking()
            .Where(a => followingIds.Contains(a.UserId));

        if (request.Cursor.HasValue)
        {
            query = query.Where(a => a.CreatedAt < request.Cursor.Value);
        }

        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Take(request.PageSize + 1)
            .Select(a => new ActivityFeedItemDto(
                a.Id,
                a.UserId,
                a.User.FullName,
                a.User.AvatarUrl,
                a.ActivityType.ToString(),
                a.TargetEntityId,
                a.Metadata,
                a.CreatedAt))
            .ToListAsync(ct);

        var hasMore = items.Count > request.PageSize;
        if (hasMore)
        {
            items.RemoveAt(items.Count - 1);
        }

        var nextCursor = hasMore && items.Count > 0
            ? items[^1].CreatedAt
            : (DateTimeOffset?)null;

        return Result<ActivityFeedResponse>.Success(
            new ActivityFeedResponse(items, nextCursor, hasMore));
    }
}
