namespace SwiggyClone.Application.Features.Restaurants.DTOs;

public sealed record MenuItemSummaryDto(
    Guid Id,
    string Name,
    int Price,
    int? DiscountedPrice,
    string? ImageUrl,
    bool IsVeg,
    bool IsAvailable,
    bool IsBestseller,
    short SpiceLevel,
    short[]? Allergens,
    short[]? DietaryTags,
    int? CalorieCount);
