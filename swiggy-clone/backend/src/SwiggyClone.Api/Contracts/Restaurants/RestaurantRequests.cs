namespace SwiggyClone.Api.Contracts.Restaurants;

public sealed record RegisterRestaurantRequest(
    string Name,
    string? Description,
    string? PhoneNumber,
    string? Email,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? State,
    string? PostalCode,
    double? Latitude,
    double? Longitude,
    bool IsVegOnly,
    int? AvgCostForTwo,
    List<Guid>? CuisineIds);

public sealed record UpdateRestaurantRequest(
    string? Name,
    string? Description,
    string? PhoneNumber,
    string? Email,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? State,
    string? PostalCode,
    double? Latitude,
    double? Longitude,
    bool? IsVegOnly,
    int? AvgCostForTwo,
    List<Guid>? CuisineIds);

public sealed record ToggleRequest(bool Value);
