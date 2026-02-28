using SwiggyClone.Application.Features.Discovery.DTOs;

namespace SwiggyClone.Application.Features.Favourites.DTOs;

public sealed record FavouriteDto(
    Guid RestaurantId,
    DateTimeOffset FavouritedAt,
    CustomerRestaurantDto Restaurant);
