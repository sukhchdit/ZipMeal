using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Social.Dtos;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Social.Queries;

public sealed record GetFollowingQuery(
    Guid UserId,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<List<FollowUserDto>>>;

internal sealed class GetFollowingQueryHandler(IAppDbContext db)
    : IRequestHandler<GetFollowingQuery, Result<List<FollowUserDto>>>
{
    public async Task<Result<List<FollowUserDto>>> Handle(
        GetFollowingQuery request, CancellationToken ct)
    {
        var following = await db.UserFollows
            .AsNoTracking()
            .Where(f => f.FollowerId == request.UserId)
            .OrderByDescending(f => f.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(f => new FollowUserDto(
                f.Following.Id,
                f.Following.FullName,
                f.Following.AvatarUrl,
                f.CreatedAt))
            .ToListAsync(ct);

        return Result<List<FollowUserDto>>.Success(following);
    }
}
