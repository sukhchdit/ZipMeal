using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Reviews.Commands;

internal sealed class ReplyToReviewCommandHandler(IAppDbContext db)
    : IRequestHandler<ReplyToReviewCommand, Result>
{
    public async Task<Result> Handle(ReplyToReviewCommand request, CancellationToken ct)
    {
        var review = await db.Reviews
            .Include(r => r.Restaurant)
            .FirstOrDefaultAsync(r => r.Id == request.ReviewId, ct);

        if (review is null)
            return Result.Failure("REVIEW_NOT_FOUND", "Review not found.");

        if (review.Restaurant.OwnerId != request.OwnerId)
            return Result.Failure("UNAUTHORIZED", "You are not the owner of this restaurant.");

        review.RestaurantReply = request.ReplyText;
        review.RepliedAt = DateTimeOffset.UtcNow;
        review.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
