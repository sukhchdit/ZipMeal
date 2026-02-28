using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Discovery.Notifications;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Reviews.Commands;

internal sealed class ToggleReviewVisibilityCommandHandler(IAppDbContext db, IPublisher publisher)
    : IRequestHandler<ToggleReviewVisibilityCommand, Result>
{
    public async Task<Result> Handle(ToggleReviewVisibilityCommand request, CancellationToken ct)
    {
        var review = await db.Reviews
            .FirstOrDefaultAsync(r => r.Id == request.ReviewId, ct);

        if (review is null)
            return Result.Failure("REVIEW_NOT_FOUND", "Review not found.");

        review.IsVisible = request.IsVisible;
        review.UpdatedAt = DateTimeOffset.UtcNow;

        // Recalculate restaurant average from visible reviews
        var restaurant = await db.Restaurants
            .FirstOrDefaultAsync(r => r.Id == review.RestaurantId, ct);

        if (restaurant is not null)
        {
            var visibleReviews = await db.Reviews
                .Where(r => r.RestaurantId == review.RestaurantId && r.IsVisible)
                .ToListAsync(ct);

            // Account for current review's new visibility state
            if (request.IsVisible && !visibleReviews.Any(r => r.Id == review.Id))
                visibleReviews.Add(review);
            else if (!request.IsVisible)
                visibleReviews.RemoveAll(r => r.Id == review.Id);

            if (visibleReviews.Count > 0)
            {
                restaurant.AverageRating = (decimal)visibleReviews.Average(r => r.Rating);
                restaurant.TotalRatings = visibleReviews.Count;
            }
            else
            {
                restaurant.AverageRating = 0;
                restaurant.TotalRatings = 0;
            }
        }

        await db.SaveChangesAsync(ct);

        // Reindex restaurant in ES
        await publisher.Publish(new RestaurantIndexRequested(review.RestaurantId), ct);

        return Result.Success();
    }
}
