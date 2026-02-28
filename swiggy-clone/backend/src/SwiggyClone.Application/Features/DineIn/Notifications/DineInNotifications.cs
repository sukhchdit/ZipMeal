using MediatR;

namespace SwiggyClone.Application.Features.DineIn.Notifications;

public sealed record DineInSessionStartedNotification(
    Guid SessionId,
    Guid RestaurantId,
    string TableNumber) : INotification;

public sealed record DineInMemberJoinedNotification(
    Guid SessionId,
    Guid UserId,
    string UserName) : INotification;

public sealed record DineInMemberLeftNotification(
    Guid SessionId,
    Guid UserId) : INotification;

public sealed record DineInOrderPlacedNotification(
    Guid SessionId,
    Guid OrderId,
    Guid UserId,
    int TotalAmount) : INotification;

public sealed record DineInOrderStatusChangedNotification(
    Guid SessionId,
    Guid OrderId,
    string NewStatus) : INotification;

public sealed record DineInBillRequestedNotification(
    Guid SessionId,
    Guid RestaurantId) : INotification;

public sealed record DineInSessionEndedNotification(
    Guid SessionId,
    Guid RestaurantId,
    string TableNumber) : INotification;
