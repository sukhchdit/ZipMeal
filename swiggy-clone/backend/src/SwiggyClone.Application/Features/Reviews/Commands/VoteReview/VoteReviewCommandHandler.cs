using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Reviews.Commands.VoteReview;

internal sealed class VoteReviewCommandHandler(IAppDbContext db)
    : IRequestHandler<VoteReviewCommand, Result>
{
    public async Task<Result> Handle(VoteReviewCommand request, CancellationToken ct)
    {
        var review = await db.Reviews
            .FirstOrDefaultAsync(r => r.Id == request.ReviewId && r.IsVisible, ct);

        if (review is null)
            return Result.Failure("REVIEW_NOT_FOUND", "Review not found.");

        var existingVote = await db.ReviewVotes
            .FirstOrDefaultAsync(v => v.ReviewId == request.ReviewId && v.UserId == request.UserId, ct);

        if (existingVote is not null)
        {
            existingVote.IsHelpful = request.IsHelpful;
        }
        else
        {
            db.ReviewVotes.Add(new ReviewVote
            {
                ReviewId = request.ReviewId,
                UserId = request.UserId,
                IsHelpful = request.IsHelpful,
                CreatedAt = DateTimeOffset.UtcNow,
            });
        }

        await db.SaveChangesAsync(ct);

        // Recalculate from DB after save
        review.HelpfulCount = await db.ReviewVotes
            .CountAsync(v => v.ReviewId == request.ReviewId && v.IsHelpful, ct);

        await db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
