namespace SwiggyClone.Application.Features.Addresses.DTOs;

public sealed record AddressDto(
    Guid Id,
    string Label,
    string AddressLine1,
    string? AddressLine2,
    string? City,
    string? State,
    string? PostalCode,
    string? Country,
    double Latitude,
    double Longitude,
    bool IsDefault,
    DateTimeOffset CreatedAt);
