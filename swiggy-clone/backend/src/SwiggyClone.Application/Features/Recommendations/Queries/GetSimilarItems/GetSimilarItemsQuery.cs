using MediatR;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Recommendations.DTOs;
using SwiggyClone.Shared;
using SwiggyClone.Shared.Constants;

namespace SwiggyClone.Application.Features.Recommendations.Queries.GetSimilarItems;

public sealed record GetSimilarItemsQuery(
    Guid MenuItemId,
    int Count = 10) : IRequest<Result<List<RecommendedMenuItemDto>>>, ICacheable
{
    public string CacheKey => CacheKeys.RecommendationsSimilarItem(MenuItemId);
    public int CacheExpirationMinutes => 60;
}
