using MediatR;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

public sealed record UpdateRestaurantProfileCommand(
    Guid RestaurantId,
    Guid OwnerId,
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
    List<Guid>? CuisineIds) : IRequest<Result<RestaurantDto>>;
