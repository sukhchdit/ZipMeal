namespace SwiggyClone.Application.Features.Deliveries.DTOs;

public sealed record DeliveryTrackingDto(
    Guid OrderId,
    int DeliveryStatus,
    string? PartnerName,
    string? PartnerPhone,
    double? CurrentLatitude,
    double? CurrentLongitude,
    DateTimeOffset? AssignedAt,
    DateTimeOffset? AcceptedAt,
    DateTimeOffset? PickedUpAt,
    DateTimeOffset? EstimatedDeliveryTime);
