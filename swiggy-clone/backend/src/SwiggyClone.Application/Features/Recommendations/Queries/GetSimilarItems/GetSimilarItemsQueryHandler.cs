using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Recommendations.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Recommendations.Queries.GetSimilarItems;

internal sealed class GetSimilarItemsQueryHandler(
    IAppDbContext db,
    IRecommendationEngine engine)
    : IRequestHandler<GetSimilarItemsQuery, Result<List<RecommendedMenuItemDto>>>
{
    public async Task<Result<List<RecommendedMenuItemDto>>> Handle(
        GetSimilarItemsQuery request,
        CancellationToken ct)
    {
        var exists = await db.MenuItems
            .AsNoTracking()
            .AnyAsync(m => m.Id == request.MenuItemId && m.IsAvailable, ct);

        if (!exists)
        {
            return Result<List<RecommendedMenuItemDto>>.Failure(
                "MENU_ITEM_NOT_FOUND",
                "Menu item not found.");
        }

        var items = await engine.GetSimilarItemsAsync(request.MenuItemId, request.Count, ct);
        return Result<List<RecommendedMenuItemDto>>.Success(items);
    }
}
