namespace SwiggyClone.Application.Features.Discovery.DTOs;

/// <summary>
/// Restaurant summary optimised for customer-facing listing cards.
/// Includes delivery time, cost, veg flag, and cuisine tags that
/// <see cref="Restaurants.DTOs.RestaurantSummaryDto"/> omits.
/// </summary>
public sealed record CustomerRestaurantDto(
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
    List<string> Cuisines);
