using MediatR;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Discovery.DTOs;
using SwiggyClone.Shared;
using SwiggyClone.Shared.Constants;

namespace SwiggyClone.Application.Features.Discovery.Queries;

public sealed record GetPublicRestaurantDetailQuery(Guid RestaurantId)
    : IRequest<Result<PublicRestaurantDetailDto>>, ICacheable
{
    public string CacheKey => CacheKeys.RestaurantDetail(RestaurantId);
    public int CacheExpirationMinutes => 10;
}
