using SwiggyClone.Application.Features.Restaurants.DTOs;

namespace SwiggyClone.Application.Features.Discovery.DTOs;

/// <summary>
/// Full restaurant detail for the customer restaurant page.
/// Excludes owner-only fields (FSSAI, GST, commission rate).
/// Includes operating hours and full menu tree.
/// </summary>
public sealed record PublicRestaurantDetailDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    string? LogoUrl,
    string? BannerUrl,
    string? AddressLine1,
    string? City,
    string? State,
    string? PostalCode,
    double? Latitude,
    double? Longitude,
    decimal AverageRating,
    int TotalRatings,
    int? AvgDeliveryTimeMin,
    int? AvgCostForTwo,
    bool IsVegOnly,
    bool IsAcceptingOrders,
    bool IsDineInEnabled,
    List<string> Cuisines,
    List<OperatingHoursDto> OperatingHours,
    List<MenuSectionDto> MenuSections);

/// <summary>
/// A menu category with its available items, used in the restaurant detail page.
/// </summary>
public sealed record MenuSectionDto(
    Guid CategoryId,
    string CategoryName,
    int SortOrder,
    List<MenuItemDto> Items);
