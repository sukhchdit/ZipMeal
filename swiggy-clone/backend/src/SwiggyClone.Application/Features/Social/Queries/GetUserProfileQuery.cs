using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Social.Dtos;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Social.Queries;

public sealed record GetUserProfileQuery(
    Guid TargetUserId,
    Guid? CurrentUserId) : IRequest<Result<UserProfileDto>>;

internal sealed class GetUserProfileQueryHandler(IAppDbContext db)
    : IRequestHandler<GetUserProfileQuery, Result<UserProfileDto>>
{
    public async Task<Result<UserProfileDto>> Handle(
        GetUserProfileQuery request, CancellationToken ct)
    {
        var user = await db.Users
            .AsNoTracking()
            .Where(u => u.Id == request.TargetUserId)
            .Select(u => new { u.Id, u.FullName, u.AvatarUrl })
            .FirstOrDefaultAsync(ct);

        if (user is null)
            return Result<UserProfileDto>.Failure("USER_NOT_FOUND", "User not found.");

        var followerCount = await db.UserFollows
            .AsNoTracking()
            .CountAsync(f => f.FollowingId == request.TargetUserId, ct);

        var followingCount = await db.UserFollows
            .AsNoTracking()
            .CountAsync(f => f.FollowerId == request.TargetUserId, ct);

        var reviewCount = await db.Reviews
            .AsNoTracking()
            .CountAsync(r => r.UserId == request.TargetUserId, ct);

        var isFollowedByCurrentUser = request.CurrentUserId.HasValue
            && await db.UserFollows
                .AsNoTracking()
                .AnyAsync(f =>
                    f.FollowerId == request.CurrentUserId.Value
                    && f.FollowingId == request.TargetUserId, ct);

        var recentActivity = await db.ActivityFeedItems
            .AsNoTracking()
            .Where(a => a.UserId == request.TargetUserId)
            .OrderByDescending(a => a.CreatedAt)
            .Take(5)
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

        return Result<UserProfileDto>.Success(new UserProfileDto(
            user.Id,
            user.FullName,
            user.AvatarUrl,
            followerCount,
            followingCount,
            reviewCount,
            isFollowedByCurrentUser,
            recentActivity));
    }
}
