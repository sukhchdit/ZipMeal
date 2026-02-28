using MediatR;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Queries;

public sealed record GetCuisineTypesQuery() : IRequest<Result<List<CuisineTypeDto>>>, ICacheable
{
    public string CacheKey => "cuisine_types_all";
    public int CacheExpirationMinutes => 60;
}
