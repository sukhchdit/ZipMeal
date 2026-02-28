using MediatR;

namespace SwiggyClone.Application.Features.Reviews.Notifications;

public sealed record ReviewSubmittedNotification(
    Guid ReviewId,
    Guid UserId,
    Guid RestaurantId) : INotification;
