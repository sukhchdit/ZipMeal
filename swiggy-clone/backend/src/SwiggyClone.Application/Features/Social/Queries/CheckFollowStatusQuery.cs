using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Social.Dtos;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Social.Queries;

public sealed record CheckFollowStatusQuery(
    Guid FollowerId,
    Guid FollowingId) : IRequest<Result<FollowStatusDto>>;

internal sealed class CheckFollowStatusQueryHandler(IAppDbContext db)
    : IRequestHandler<CheckFollowStatusQuery, Result<FollowStatusDto>>
{
    public async Task<Result<FollowStatusDto>> Handle(
        CheckFollowStatusQuery request, CancellationToken ct)
    {
        var isFollowing = await db.UserFollows
            .AsNoTracking()
            .AnyAsync(f =>
                f.FollowerId == request.FollowerId
                && f.FollowingId == request.FollowingId, ct);

        return Result<FollowStatusDto>.Success(new FollowStatusDto(isFollowing));
    }
}
