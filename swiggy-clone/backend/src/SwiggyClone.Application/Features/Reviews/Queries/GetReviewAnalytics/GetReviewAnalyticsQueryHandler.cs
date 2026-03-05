using System.Globalization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Reviews.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Reviews.Queries.GetReviewAnalytics;

internal sealed class GetReviewAnalyticsQueryHandler(IAppDbContext db)
    : IRequestHandler<GetReviewAnalyticsQuery, Result<ReviewAnalyticsDto>>
{
    public async Task<Result<ReviewAnalyticsDto>> Handle(GetReviewAnalyticsQuery request, CancellationToken ct)
    {
        var restaurant = await db.Restaurants
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == request.RestaurantId, ct);

        if (restaurant is null)
            return Result<ReviewAnalyticsDto>.Failure("RESTAURANT_NOT_FOUND", "Restaurant not found.");

        if (restaurant.OwnerId != request.OwnerId)
            return Result<ReviewAnalyticsDto>.Failure("UNAUTHORIZED", "You are not the owner of this restaurant.");

        var visibleReviews = db.Reviews
            .AsNoTracking()
            .Where(r => r.RestaurantId == request.RestaurantId && r.IsVisible);

        var totalReviews = await visibleReviews.CountAsync(ct);

        var averageRating = totalReviews > 0
            ? Math.Round(await visibleReviews.AverageAsync(r => (decimal)r.Rating, ct), 1)
            : 0m;

        var photoReviewsCount = await visibleReviews
            .CountAsync(r => r.Photos.Any(), ct);

        decimal? averageDeliveryRating = null;
        var hasDeliveryRatings = await visibleReviews
            .AnyAsync(r => r.DeliveryRating != null, ct);

        if (hasDeliveryRatings)
        {
            averageDeliveryRating = Math.Round(
                await visibleReviews
                    .Where(r => r.DeliveryRating != null)
                    .AverageAsync(r => (decimal)r.DeliveryRating!.Value, ct),
                1);
        }

        // Rating distribution (1-5)
        var distributionRaw = await visibleReviews
            .GroupBy(r => r.Rating)
            .Select(g => new { Rating = (int)g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var ratingDistribution = new Dictionary<int, int>
        {
            { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 },
        };
        foreach (var item in distributionRaw)
        {
            ratingDistribution[item.Rating] = item.Count;
        }

        // Monthly trend (last 6 months)
        var sixMonthsAgo = DateTimeOffset.UtcNow.AddMonths(-6);

        var trendRaw = await visibleReviews
            .Where(r => r.CreatedAt >= sixMonthsAgo)
            .GroupBy(r => new { r.CreatedAt.Year, r.CreatedAt.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count(), Avg = g.Average(r => (decimal)r.Rating) })
            .OrderBy(g => g.Year).ThenBy(g => g.Month)
            .ToListAsync(ct);

        var trend = trendRaw.Select(t => new MonthlyTrendItem(
            $"{t.Year.ToString(CultureInfo.InvariantCulture)}-{t.Month.ToString("D2", CultureInfo.InvariantCulture)}",
            t.Count,
            Math.Round(t.Avg, 1)))
            .ToList();

        return Result<ReviewAnalyticsDto>.Success(new ReviewAnalyticsDto(
            averageRating,
            totalReviews,
            photoReviewsCount,
            averageDeliveryRating,
            ratingDistribution,
            trend));
    }
}
