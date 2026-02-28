using MediatR;
using Microsoft.Extensions.Logging;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Discovery.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Discovery.Queries;

internal sealed class SearchMenuItemsQueryHandler(
    ISearchService searchService,
    ILogger<SearchMenuItemsQueryHandler> logger)
    : IRequestHandler<SearchMenuItemsQuery, Result<List<MenuItemSearchGroupedResultDto>>>
{
    public async Task<Result<List<MenuItemSearchGroupedResultDto>>> Handle(
        SearchMenuItemsQuery request, CancellationToken ct)
    {
        var term = request.Term.Trim();
        var pageSize = Math.Clamp(request.PageSize, 1, 50);

        if (!await searchService.IsAvailableAsync(ct))
            return Result<List<MenuItemSearchGroupedResultDto>>.Failure(
                "SEARCH_UNAVAILABLE", "Search service is currently unavailable. Please try again later.");

        try
        {
            var esResults = await searchService.SearchMenuItemsAsync(term, request.City, pageSize, ct);

            var grouped = esResults
                .GroupBy(r => r.RestaurantId)
                .Select(g =>
                {
                    var first = g.First();
                    return new MenuItemSearchGroupedResultDto(
                        first.RestaurantId,
                        first.RestaurantName,
                        first.RestaurantSlug,
                        first.RestaurantLogoUrl,
                        first.RestaurantCity,
                        first.RestaurantAverageRating,
                        first.RestaurantTotalRatings,
                        first.RestaurantIsAcceptingOrders,
                        g.Select(i => new MenuItemSearchHitDto(
                            i.Id, i.Name, i.Description, i.Price,
                            i.DiscountedPrice, i.ImageUrl, i.IsVeg, i.IsBestseller
                        )).ToList());
                })
                .ToList();

            return Result<List<MenuItemSearchGroupedResultDto>>.Success(grouped);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Menu item search failed");
            return Result<List<MenuItemSearchGroupedResultDto>>.Failure(
                "SEARCH_FAILED", "Search failed. Please try again.");
        }
    }
}
