using MediatR;

namespace SwiggyClone.Application.Features.Favourites.Notifications;

public sealed record RestaurantFavouritedNotification(
    Guid UserId,
    Guid RestaurantId) : INotification;
