using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Recommendations.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Recommendations.Queries.GetSimilarRestaurants;

internal sealed class GetSimilarRestaurantsQueryHandler(
    IAppDbContext db,
    IRecommendationEngine engine)
    : IRequestHandler<GetSimilarRestaurantsQuery, Result<List<RecommendedRestaurantDto>>>
{
    public async Task<Result<List<RecommendedRestaurantDto>>> Handle(
        GetSimilarRestaurantsQuery request,
        CancellationToken ct)
    {
        var exists = await db.Restaurants
            .AsNoTracking()
            .AnyAsync(r => r.Id == request.RestaurantId && r.Status == RestaurantStatus.Approved, ct);

        if (!exists)
        {
            return Result<List<RecommendedRestaurantDto>>.Failure(
                "RESTAURANT_NOT_FOUND",
                "Restaurant not found.");
        }

        var items = await engine.GetSimilarRestaurantsAsync(request.RestaurantId, request.Count, ct);
        return Result<List<RecommendedRestaurantDto>>.Success(items);
    }
}
