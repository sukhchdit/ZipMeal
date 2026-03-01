using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Social.Commands;

public sealed record UnfollowUserCommand(Guid FollowerId, Guid FollowingId) : IRequest<Result>;

internal sealed class UnfollowUserCommandHandler(IAppDbContext db)
    : IRequestHandler<UnfollowUserCommand, Result>
{
    public async Task<Result> Handle(UnfollowUserCommand request, CancellationToken ct)
    {
        var follow = await db.UserFollows
            .FirstOrDefaultAsync(
                f => f.FollowerId == request.FollowerId && f.FollowingId == request.FollowingId, ct);

        if (follow is null)
            return Result.Failure("NOT_FOLLOWING", "You are not following this user.");

        db.UserFollows.Remove(follow);
        await db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
