using MediatR;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Shared;
using SwiggyClone.Shared.Constants;

namespace SwiggyClone.Application.Features.Restaurants.Queries;

public sealed record GetCuisineTypesQuery() : IRequest<Result<List<CuisineTypeDto>>>, ICacheable
{
    public string CacheKey => CacheKeys.CuisineTypesKey;
    public int CacheExpirationMinutes => 60;
}
