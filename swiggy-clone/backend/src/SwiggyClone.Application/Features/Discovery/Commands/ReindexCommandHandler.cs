using System.Diagnostics;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Discovery.Documents;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Discovery.Commands;

internal sealed class ReindexCommandHandler(
    ISearchService searchService,
    IAppDbContext db,
    ILogger<ReindexCommandHandler> logger)
    : IRequestHandler<ReindexCommand, Result<ReindexResultDto>>
{
    public async Task<Result<ReindexResultDto>> Handle(
        ReindexCommand request, CancellationToken ct)
    {
        if (!await searchService.IsAvailableAsync(ct))
            return Result<ReindexResultDto>.Failure(
                "SEARCH_UNAVAILABLE", "Elasticsearch is not available.");

        var sw = Stopwatch.StartNew();

        if (request.Recreate)
            await searchService.RecreateIndicesAsync(ct);
        else
            await searchService.EnsureIndicesCreatedAsync(ct);

        // Bulk index restaurants (approved only, not soft-deleted)
        var restaurants = await db.Restaurants
            .AsNoTracking()
            .Where(r => r.Status == RestaurantStatus.Approved && !r.IsDeleted)
            .Include(r => r.RestaurantCuisines)
                .ThenInclude(rc => rc.CuisineType)
            .ToListAsync(ct);

        var restaurantDocs = restaurants.Select(r => new RestaurantSearchDocument
        {
            Id = r.Id,
            Name = r.Name,
            Slug = r.Slug,
            Description = r.Description,
            LogoUrl = r.LogoUrl,
            BannerUrl = r.BannerUrl,
            City = r.City?.ToLowerInvariant(),
            State = r.State,
            AverageRating = r.AverageRating,
            TotalRatings = r.TotalRatings,
            AvgDeliveryTimeMin = r.AvgDeliveryTimeMin,
            AvgCostForTwo = r.AvgCostForTwo,
            IsVegOnly = r.IsVegOnly,
            IsAcceptingOrders = r.IsAcceptingOrders,
            IsDineInEnabled = r.IsDineInEnabled,
            Cuisines = r.RestaurantCuisines.Select(rc => rc.CuisineType.Name).ToList(),
            NameSuggest = r.Name,
        });

        await searchService.BulkIndexRestaurantsAsync(restaurantDocs, ct);

        // Bulk index menu items (from approved restaurants, not soft-deleted)
        var approvedRestaurantIds = restaurants.Select(r => r.Id).ToHashSet();

        var menuItems = await db.MenuItems
            .AsNoTracking()
            .Where(mi => !mi.IsDeleted && approvedRestaurantIds.Contains(mi.RestaurantId))
            .Include(mi => mi.Restaurant)
            .ToListAsync(ct);

        var menuItemDocs = menuItems.Select(mi => new MenuItemSearchDocument
        {
            Id = mi.Id,
            Name = mi.Name,
            Description = mi.Description,
            Price = mi.Price,
            DiscountedPrice = mi.DiscountedPrice,
            ImageUrl = mi.ImageUrl,
            IsVeg = mi.IsVeg,
            IsAvailable = mi.IsAvailable,
            IsBestseller = mi.IsBestseller,
            RestaurantId = mi.RestaurantId,
            RestaurantName = mi.Restaurant.Name,
            RestaurantSlug = mi.Restaurant.Slug,
            RestaurantLogoUrl = mi.Restaurant.LogoUrl,
            RestaurantCity = mi.Restaurant.City?.ToLowerInvariant(),
            RestaurantAverageRating = mi.Restaurant.AverageRating,
            RestaurantTotalRatings = mi.Restaurant.TotalRatings,
            RestaurantIsAcceptingOrders = mi.Restaurant.IsAcceptingOrders,
            NameSuggest = mi.Name,
        });

        await searchService.BulkIndexMenuItemsAsync(menuItemDocs, ct);

        sw.Stop();

        logger.LogInformation(
            "Reindex completed: {Restaurants} restaurants, {MenuItems} menu items in {Elapsed}ms",
            restaurants.Count, menuItems.Count, sw.ElapsedMilliseconds);

        return Result<ReindexResultDto>.Success(
            new ReindexResultDto(restaurants.Count, menuItems.Count, sw.ElapsedMilliseconds));
    }
}
