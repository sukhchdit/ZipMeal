using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Recommendations.DTOs;
using SwiggyClone.Shared;
using SwiggyClone.Shared.Constants;

namespace SwiggyClone.Application.Features.Recommendations.Queries.GetPersonalizedRecommendations;

internal sealed class GetPersonalizedRecommendationsQueryHandler(
    IRecommendationEngine engine,
    IDistributedCache cache)
    : IRequestHandler<GetPersonalizedRecommendationsQuery, Result<PersonalizedRecommendationsDto>>
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(4);

    public async Task<Result<PersonalizedRecommendationsDto>> Handle(
        GetPersonalizedRecommendationsQuery request,
        CancellationToken ct)
    {
        var cacheKey = CacheKeys.RecommendationsPersonalized(request.UserId);

        var cached = await cache.GetStringAsync(cacheKey, ct);
        if (cached is not null)
        {
            var dto = JsonSerializer.Deserialize<PersonalizedRecommendationsDto>(cached);
            if (dto is not null)
            {
                return Result<PersonalizedRecommendationsDto>.Success(dto);
            }
        }

        var result = await engine.GetPersonalizedAsync(
            request.UserId,
            request.City,
            request.MaxRestaurants,
            request.MaxItems,
            ct);

        var json = JsonSerializer.Serialize(result);
        await cache.SetStringAsync(
            cacheKey,
            json,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = CacheTtl },
            ct);

        return Result<PersonalizedRecommendationsDto>.Success(result);
    }
}
