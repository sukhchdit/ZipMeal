using MediatR;

namespace SwiggyClone.Application.Features.Orders.Notifications;

public sealed record OrderPlacedNotification(
    Guid OrderId,
    Guid UserId,
    Guid RestaurantId,
    string OrderNumber,
    int TotalAmount) : INotification;

public sealed record OrderStatusChangedNotification(
    Guid OrderId,
    Guid UserId,
    string NewStatus,
    string OrderNumber) : INotification;

public sealed record OrderCancelledNotification(
    Guid OrderId,
    Guid UserId,
    string? Reason,
    bool RefundInitiated) : INotification;
