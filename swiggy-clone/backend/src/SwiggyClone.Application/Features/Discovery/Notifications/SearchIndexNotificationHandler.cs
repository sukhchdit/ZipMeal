using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Discovery.Documents;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Application.Features.Discovery.Notifications;

internal sealed class SearchIndexNotificationHandler(
    ISearchService searchService,
    IAppDbContext db,
    ILogger<SearchIndexNotificationHandler> logger)
    : INotificationHandler<RestaurantIndexRequested>,
      INotificationHandler<RestaurantDeleteFromIndexRequested>,
      INotificationHandler<MenuItemIndexRequested>,
      INotificationHandler<MenuItemDeleteFromIndexRequested>
{
    public async Task Handle(RestaurantIndexRequested notification, CancellationToken ct)
    {
        if (!await searchService.IsAvailableAsync(ct)) return;

        try
        {
            var restaurant = await db.Restaurants
                .AsNoTracking()
                .Include(r => r.RestaurantCuisines).ThenInclude(rc => rc.CuisineType)
                .FirstOrDefaultAsync(r => r.Id == notification.RestaurantId && !r.IsDeleted, ct);

            if (restaurant is null || restaurant.Status != RestaurantStatus.Approved)
            {
                await searchService.DeleteRestaurantAsync(notification.RestaurantId, ct);
                return;
            }

            await searchService.IndexRestaurantAsync(new RestaurantSearchDocument
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Slug = restaurant.Slug,
                Description = restaurant.Description,
                LogoUrl = restaurant.LogoUrl,
                BannerUrl = restaurant.BannerUrl,
                City = restaurant.City?.ToLowerInvariant(),
                State = restaurant.State,
                AverageRating = restaurant.AverageRating,
                TotalRatings = restaurant.TotalRatings,
                AvgDeliveryTimeMin = restaurant.AvgDeliveryTimeMin,
                AvgCostForTwo = restaurant.AvgCostForTwo,
                IsVegOnly = restaurant.IsVegOnly,
                IsAcceptingOrders = restaurant.IsAcceptingOrders,
                IsDineInEnabled = restaurant.IsDineInEnabled,
                Cuisines = restaurant.RestaurantCuisines.Select(rc => rc.CuisineType.Name).ToList(),
                NameSuggest = restaurant.Name,
            }, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to index restaurant {Id}", notification.RestaurantId);
        }
    }

    public async Task Handle(RestaurantDeleteFromIndexRequested notification, CancellationToken ct)
    {
        if (!await searchService.IsAvailableAsync(ct)) return;

        try
        {
            await searchService.DeleteRestaurantAsync(notification.RestaurantId, ct);
            await searchService.DeleteMenuItemsByRestaurantAsync(notification.RestaurantId, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to delete restaurant {Id} from index", notification.RestaurantId);
        }
    }

    public async Task Handle(MenuItemIndexRequested notification, CancellationToken ct)
    {
        if (!await searchService.IsAvailableAsync(ct)) return;

        try
        {
            var menuItem = await db.MenuItems
                .AsNoTracking()
                .Include(mi => mi.Restaurant)
                .FirstOrDefaultAsync(mi => mi.Id == notification.MenuItemId && !mi.IsDeleted, ct);

            if (menuItem is null || menuItem.Restaurant.Status != RestaurantStatus.Approved)
            {
                await searchService.DeleteMenuItemAsync(notification.MenuItemId, ct);
                return;
            }

            await searchService.IndexMenuItemAsync(new MenuItemSearchDocument
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                DiscountedPrice = menuItem.DiscountedPrice,
                ImageUrl = menuItem.ImageUrl,
                IsVeg = menuItem.IsVeg,
                IsAvailable = menuItem.IsAvailable,
                IsBestseller = menuItem.IsBestseller,
                RestaurantId = menuItem.RestaurantId,
                RestaurantName = menuItem.Restaurant.Name,
                RestaurantSlug = menuItem.Restaurant.Slug,
                RestaurantLogoUrl = menuItem.Restaurant.LogoUrl,
                RestaurantCity = menuItem.Restaurant.City?.ToLowerInvariant(),
                RestaurantAverageRating = menuItem.Restaurant.AverageRating,
                RestaurantTotalRatings = menuItem.Restaurant.TotalRatings,
                RestaurantIsAcceptingOrders = menuItem.Restaurant.IsAcceptingOrders,
                NameSuggest = menuItem.Name,
            }, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to index menu item {Id}", notification.MenuItemId);
        }
    }

    public async Task Handle(MenuItemDeleteFromIndexRequested notification, CancellationToken ct)
    {
        if (!await searchService.IsAvailableAsync(ct)) return;

        try
        {
            await searchService.DeleteMenuItemAsync(notification.MenuItemId, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to delete menu item {Id} from index", notification.MenuItemId);
        }
    }
}
