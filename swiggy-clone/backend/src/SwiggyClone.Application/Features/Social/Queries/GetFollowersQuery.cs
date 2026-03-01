using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Social.Dtos;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Social.Queries;

public sealed record GetFollowersQuery(
    Guid UserId,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<List<FollowUserDto>>>;

internal sealed class GetFollowersQueryHandler(IAppDbContext db)
    : IRequestHandler<GetFollowersQuery, Result<List<FollowUserDto>>>
{
    public async Task<Result<List<FollowUserDto>>> Handle(
        GetFollowersQuery request, CancellationToken ct)
    {
        var followers = await db.UserFollows
            .AsNoTracking()
            .Where(f => f.FollowingId == request.UserId)
            .OrderByDescending(f => f.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(f => new FollowUserDto(
                f.Follower.Id,
                f.Follower.FullName,
                f.Follower.AvatarUrl,
                f.CreatedAt))
            .ToListAsync(ct);

        return Result<List<FollowUserDto>>.Success(followers);
    }
}
