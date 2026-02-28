namespace SwiggyClone.Application.Features.Deliveries.DTOs;

public sealed record DeliveryAssignmentDto(
    Guid Id,
    Guid OrderId,
    string OrderNumber,
    string RestaurantName,
    string? RestaurantAddress,
    string? CustomerAddress,
    int Status,
    DateTimeOffset AssignedAt,
    DateTimeOffset? AcceptedAt,
    DateTimeOffset? PickedUpAt,
    DateTimeOffset? DeliveredAt,
    decimal? DistanceKm,
    int Earnings);
