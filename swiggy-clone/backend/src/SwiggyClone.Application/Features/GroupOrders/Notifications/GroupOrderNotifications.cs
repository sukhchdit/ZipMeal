using MediatR;

namespace SwiggyClone.Application.Features.GroupOrders.Notifications;

public sealed record GroupOrderCreatedNotification(
    Guid GroupOrderId,
    Guid RestaurantId,
    Guid InitiatorUserId,
    string InviteCode) : INotification;

public sealed record GroupOrderParticipantJoinedNotification(
    Guid GroupOrderId,
    Guid UserId,
    string UserName) : INotification;

public sealed record GroupOrderParticipantLeftNotification(
    Guid GroupOrderId,
    Guid UserId) : INotification;

public sealed record GroupOrderParticipantReadyNotification(
    Guid GroupOrderId,
    Guid UserId) : INotification;

public sealed record GroupOrderCartUpdatedNotification(
    Guid GroupOrderId,
    Guid UserId,
    int ItemCount,
    int ItemsTotal) : INotification;

public sealed record GroupOrderFinalizedNotification(
    Guid GroupOrderId,
    Guid OrderId,
    int TotalAmount) : INotification;

public sealed record GroupOrderCancelledNotification(
    Guid GroupOrderId) : INotification;

public sealed record GroupOrderExpiredNotification(
    Guid GroupOrderId) : INotification;
