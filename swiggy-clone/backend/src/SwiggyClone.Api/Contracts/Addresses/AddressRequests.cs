namespace SwiggyClone.Api.Contracts.Addresses;

public sealed record CreateAddressRequest(
    string Label,
    string AddressLine1,
    string? AddressLine2,
    string? City,
    string? State,
    string? PostalCode,
    string? Country,
    double Latitude,
    double Longitude,
    bool IsDefault);

public sealed record UpdateAddressRequest(
    string Label,
    string AddressLine1,
    string? AddressLine2,
    string? City,
    string? State,
    string? PostalCode,
    string? Country,
    double Latitude,
    double Longitude);
