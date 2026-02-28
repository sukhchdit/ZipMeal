namespace SwiggyClone.Application.Features.Restaurants.DTOs;

public sealed record MenuItemAddonDto(
    Guid Id,
    string Name,
    int Price,
    bool IsVeg,
    bool IsAvailable,
    int MaxQuantity,
    int SortOrder);
