using MediatR;

namespace SwiggyClone.Application.Features.Discovery.Notifications;

public sealed record RestaurantIndexRequested(Guid RestaurantId) : INotification;
public sealed record RestaurantDeleteFromIndexRequested(Guid RestaurantId) : INotification;
public sealed record MenuItemIndexRequested(Guid MenuItemId) : INotification;
public sealed record MenuItemDeleteFromIndexRequested(Guid MenuItemId) : INotification;
