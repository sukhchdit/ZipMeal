using MediatR;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Recommendations.DTOs;
using SwiggyClone.Shared;
using SwiggyClone.Shared.Constants;

namespace SwiggyClone.Application.Features.Recommendations.Queries.GetTrendingItems;

public sealed record GetTrendingItemsQuery(
    string? City = null,
    int Count = 20) : IRequest<Result<List<TrendingItemDto>>>, ICacheable
{
    public string CacheKey => CacheKeys.RecommendationsTrending(City);
    public int CacheExpirationMinutes => 15;
}
