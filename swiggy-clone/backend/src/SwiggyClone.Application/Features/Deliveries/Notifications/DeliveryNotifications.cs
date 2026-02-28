using MediatR;

namespace SwiggyClone.Application.Features.Deliveries.Notifications;

public sealed record DeliveryAssignedNotification(
    Guid AssignmentId,
    Guid OrderId,
    Guid PartnerId,
    Guid UserId) : INotification;

public sealed record DeliveryStatusChangedNotification(
    Guid AssignmentId,
    Guid OrderId,
    Guid UserId,
    string NewStatus) : INotification;

public sealed record DeliveryLocationUpdatedNotification(
    Guid OrderId,
    Guid PartnerId,
    double Latitude,
    double Longitude,
    double? Heading,
    double? Speed) : INotification;
