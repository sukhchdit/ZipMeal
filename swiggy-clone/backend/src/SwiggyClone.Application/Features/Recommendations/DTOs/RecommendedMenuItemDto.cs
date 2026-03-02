namespace SwiggyClone.Application.Features.Recommendations.DTOs;

public sealed record RecommendedMenuItemDto(
    Guid Id,
    string Name,
    string? Description,
    int Price,
    int? DiscountedPrice,
    string? ImageUrl,
    bool IsVeg,
    bool IsBestseller,
    Guid RestaurantId,
    string RestaurantName,
    string RestaurantSlug,
    string? RecommendationReason,
    double Score);
