namespace SwiggyClone.Application.Features.Restaurants.DTOs;

public sealed record MenuItemDto(
    Guid Id,
    Guid CategoryId,
    string Name,
    string? Description,
    int Price,
    int? DiscountedPrice,
    string? ImageUrl,
    bool IsVeg,
    bool IsAvailable,
    bool IsBestseller,
    int PreparationTimeMin,
    int SortOrder,
    List<MenuItemVariantDto> Variants,
    List<MenuItemAddonDto> Addons);
