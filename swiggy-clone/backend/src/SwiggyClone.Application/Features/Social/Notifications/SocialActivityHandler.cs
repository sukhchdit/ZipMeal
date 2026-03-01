using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Reviews.Notifications;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Application.Features.Social.Notifications;

internal sealed class SocialActivityHandler(
    IAppDbContext db,
    ILogger<SocialActivityHandler> logger)
    : INotificationHandler<ReviewSubmittedNotification>
{
    public async Task Handle(ReviewSubmittedNotification notification, CancellationToken ct)
    {
        try
        {
            var feedItem = new ActivityFeedItem
            {
                Id = Guid.CreateVersion7(),
                UserId = notification.UserId,
                ActivityType = ActivityType.ReviewSubmitted,
                TargetEntityId = notification.ReviewId,
                Metadata = JsonSerializer.Serialize(new { notification.RestaurantId }),
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
            };

            db.ActivityFeedItems.Add(feedItem);
            await db.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "Failed to create activity feed item for ReviewSubmitted {ReviewId}",
                notification.ReviewId);
        }
    }
}
