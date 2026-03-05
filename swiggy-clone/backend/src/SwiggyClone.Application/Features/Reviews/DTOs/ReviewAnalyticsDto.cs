namespace SwiggyClone.Application.Features.Reviews.DTOs;

public sealed record ReviewAnalyticsDto(
    decimal AverageRating,
    int TotalReviews,
    int PhotoReviewsCount,
    decimal? AverageDeliveryRating,
    Dictionary<int, int> RatingDistribution,
    List<MonthlyTrendItem> MonthlyTrend);

public sealed record MonthlyTrendItem(
    string Month,
    int Count,
    decimal AvgRating);
