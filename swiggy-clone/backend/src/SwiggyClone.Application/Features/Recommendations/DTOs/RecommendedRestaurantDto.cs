namespace SwiggyClone.Application.Features.Recommendations.DTOs;

public sealed record RecommendedRestaurantDto(
    Guid Id,
    string Name,
    string Slug,
    string? LogoUrl,
    string? BannerUrl,
    string? City,
    decimal AverageRating,
    int TotalRatings,
    int? AvgDeliveryTimeMin,
    int? AvgCostForTwo,
    bool IsVegOnly,
    bool IsAcceptingOrders,
    bool IsDineInEnabled,
    List<string> Cuisines,
    string? RecommendationReason,
    double Score);
