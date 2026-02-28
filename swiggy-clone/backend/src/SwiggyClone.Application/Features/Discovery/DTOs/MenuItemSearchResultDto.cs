namespace SwiggyClone.Application.Features.Discovery.DTOs;

/// <summary>
/// Menu item search results grouped by restaurant for a cleaner UI.
/// </summary>
public sealed record MenuItemSearchGroupedResultDto(
    Guid RestaurantId,
    string RestaurantName,
    string RestaurantSlug,
    string? RestaurantLogoUrl,
    string? RestaurantCity,
    decimal RestaurantAverageRating,
    int RestaurantTotalRatings,
    bool RestaurantIsAcceptingOrders,
    List<MenuItemSearchHitDto> Items);

/// <summary>
/// Individual menu item within a grouped search result.
/// </summary>
public sealed record MenuItemSearchHitDto(
    Guid Id,
    string Name,
    string? Description,
    int Price,
    int? DiscountedPrice,
    string? ImageUrl,
    bool IsVeg,
    bool IsBestseller);
