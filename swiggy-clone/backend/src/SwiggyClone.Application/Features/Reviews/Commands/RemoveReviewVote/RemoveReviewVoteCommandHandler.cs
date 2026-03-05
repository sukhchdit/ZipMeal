using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Reviews.Commands.RemoveReviewVote;

internal sealed class RemoveReviewVoteCommandHandler(IAppDbContext db)
    : IRequestHandler<RemoveReviewVoteCommand, Result>
{
    public async Task<Result> Handle(RemoveReviewVoteCommand request, CancellationToken ct)
    {
        var vote = await db.ReviewVotes
            .FirstOrDefaultAsync(v => v.ReviewId == request.ReviewId && v.UserId == request.UserId, ct);

        if (vote is null)
            return Result.Failure("VOTE_NOT_FOUND", "Vote not found.");

        db.ReviewVotes.Remove(vote);
        await db.SaveChangesAsync(ct);

        // Recalculate helpful count
        var review = await db.Reviews
            .FirstOrDefaultAsync(r => r.Id == request.ReviewId, ct);

        if (review is not null)
        {
            review.HelpfulCount = await db.ReviewVotes
                .CountAsync(v => v.ReviewId == request.ReviewId && v.IsHelpful, ct);

            await db.SaveChangesAsync(ct);
        }

        return Result.Success();
    }
}
