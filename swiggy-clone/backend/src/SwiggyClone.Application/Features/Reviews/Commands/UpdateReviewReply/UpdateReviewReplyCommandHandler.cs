using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Reviews.Commands.UpdateReviewReply;

internal sealed class UpdateReviewReplyCommandHandler(IAppDbContext db)
    : IRequestHandler<UpdateReviewReplyCommand, Result>
{
    public async Task<Result> Handle(UpdateReviewReplyCommand request, CancellationToken ct)
    {
        var review = await db.Reviews
            .Include(r => r.Restaurant)
            .FirstOrDefaultAsync(r => r.Id == request.ReviewId, ct);

        if (review is null)
            return Result.Failure("REVIEW_NOT_FOUND", "Review not found.");

        if (review.Restaurant.OwnerId != request.OwnerId)
            return Result.Failure("UNAUTHORIZED", "You are not the owner of this restaurant.");

        if (review.RestaurantReply is null)
            return Result.Failure("NO_REPLY_TO_UPDATE", "No existing reply to update.");

        review.RestaurantReply = request.ReplyText;
        review.RepliedAt = DateTimeOffset.UtcNow;
        review.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
