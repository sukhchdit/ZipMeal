namespace SwiggyClone.Application.Features.Restaurants.DTOs;

public sealed record MenuItemVariantDto(
    Guid Id,
    string Name,
    int PriceAdjustment,
    bool IsDefault,
    bool IsAvailable,
    int SortOrder);
