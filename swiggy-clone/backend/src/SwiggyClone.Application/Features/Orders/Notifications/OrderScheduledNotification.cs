using MediatR;

namespace SwiggyClone.Application.Features.Orders.Notifications;

public sealed record OrderScheduledNotification(
    Guid OrderId,
    Guid UserId,
    string OrderNumber,
    int TotalAmount,
    DateTimeOffset ScheduledDeliveryTime) : INotification;
