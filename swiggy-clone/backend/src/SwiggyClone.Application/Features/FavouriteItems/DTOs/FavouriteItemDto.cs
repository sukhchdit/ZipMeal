namespace SwiggyClone.Application.Features.FavouriteItems.DTOs;

public sealed record FavouriteItemDto(
    Guid MenuItemId,
    string ItemName,
    string? ImageUrl,
    int Price,
    int? DiscountedPrice,
    bool IsVeg,
    bool IsAvailable,
    Guid RestaurantId,
    string RestaurantName,
    DateTimeOffset FavouritedAt);
