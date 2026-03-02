using MediatR;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Recommendations.DTOs;
using SwiggyClone.Shared;
using SwiggyClone.Shared.Constants;

namespace SwiggyClone.Application.Features.Recommendations.Queries.GetSimilarRestaurants;

public sealed record GetSimilarRestaurantsQuery(
    Guid RestaurantId,
    int Count = 10) : IRequest<Result<List<RecommendedRestaurantDto>>>, ICacheable
{
    public string CacheKey => CacheKeys.RecommendationsSimilarRestaurant(RestaurantId);
    public int CacheExpirationMinutes => 60;
}
