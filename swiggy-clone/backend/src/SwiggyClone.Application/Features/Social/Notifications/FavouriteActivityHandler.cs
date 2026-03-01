using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Favourites.Notifications;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Application.Features.Social.Notifications;

internal sealed class FavouriteActivityHandler(
    IAppDbContext db,
    ILogger<FavouriteActivityHandler> logger)
    : INotificationHandler<RestaurantFavouritedNotification>
{
    public async Task Handle(RestaurantFavouritedNotification notification, CancellationToken ct)
    {
        try
        {
            var restaurantName = await db.Restaurants
                .AsNoTracking()
                .Where(r => r.Id == notification.RestaurantId)
                .Select(r => r.Name)
                .FirstOrDefaultAsync(ct);

            var feedItem = new ActivityFeedItem
            {
                Id = Guid.CreateVersion7(),
                UserId = notification.UserId,
                ActivityType = ActivityType.RestaurantFavourited,
                TargetEntityId = notification.RestaurantId,
                Metadata = JsonSerializer.Serialize(new { RestaurantName = restaurantName }),
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
            };

            db.ActivityFeedItems.Add(feedItem);
            await db.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "Failed to create activity feed item for RestaurantFavourited {RestaurantId}",
                notification.RestaurantId);
        }
    }
}
