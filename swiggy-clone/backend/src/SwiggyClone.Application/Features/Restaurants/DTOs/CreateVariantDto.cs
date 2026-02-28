namespace SwiggyClone.Application.Features.Restaurants.DTOs;

public sealed record CreateVariantDto(
    string Name,
    int PriceAdjustment,
    bool IsDefault,
    bool IsAvailable,
    int SortOrder);
