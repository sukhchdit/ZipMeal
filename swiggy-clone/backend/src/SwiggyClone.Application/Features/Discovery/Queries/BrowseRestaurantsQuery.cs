using MediatR;
using SwiggyClone.Application.Features.Discovery.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Discovery.Queries;

public sealed record BrowseRestaurantsQuery(
    string? City,
    Guid? CuisineId,
    bool? IsVegOnly,
    decimal? MinRating,
    int? MaxCostForTwo,
    string? SortBy,           // "rating", "deliveryTime", "costLowToHigh", "costHighToLow"
    string? Cursor,
    int PageSize = 20) : IRequest<Result<CursorPagedResult<CustomerRestaurantDto>>>;
