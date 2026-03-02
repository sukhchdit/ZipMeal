namespace SwiggyClone.Application.Features.Recommendations.DTOs;

public sealed record TrendingItemDto(
    Guid Id,
    string Name,
    string? ImageUrl,
    int Price,
    bool IsVeg,
    bool IsBestseller,
    Guid RestaurantId,
    string RestaurantName,
    int OrderCount,
    int TrendRank);
