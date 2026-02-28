namespace SwiggyClone.Application.Features.Deliveries.DTOs;

public sealed record PartnerLocationDto(
    double Latitude,
    double Longitude,
    double? Heading,
    double? Speed,
    bool IsOnline,
    DateTimeOffset UpdatedAt);
