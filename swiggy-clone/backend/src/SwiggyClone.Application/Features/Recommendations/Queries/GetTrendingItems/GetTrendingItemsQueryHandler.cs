using MediatR;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Recommendations.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Recommendations.Queries.GetTrendingItems;

internal sealed class GetTrendingItemsQueryHandler(IRecommendationEngine engine)
    : IRequestHandler<GetTrendingItemsQuery, Result<List<TrendingItemDto>>>
{
    public async Task<Result<List<TrendingItemDto>>> Handle(
        GetTrendingItemsQuery request,
        CancellationToken ct)
    {
        var items = await engine.GetTrendingAsync(request.City, request.Count, ct);
        return Result<List<TrendingItemDto>>.Success(items);
    }
}
