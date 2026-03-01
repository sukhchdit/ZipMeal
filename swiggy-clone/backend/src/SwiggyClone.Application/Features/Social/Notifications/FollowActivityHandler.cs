using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Application.Features.Social.Notifications;

internal sealed class FollowActivityHandler(
    IAppDbContext db,
    ILogger<FollowActivityHandler> logger)
    : INotificationHandler<UserFollowedNotification>
{
    public async Task Handle(UserFollowedNotification notification, CancellationToken ct)
    {
        try
        {
            var followingName = await db.Users
                .AsNoTracking()
                .Where(u => u.Id == notification.FollowingId)
                .Select(u => u.FullName)
                .FirstOrDefaultAsync(ct);

            var feedItem = new ActivityFeedItem
            {
                Id = Guid.CreateVersion7(),
                UserId = notification.FollowerId,
                ActivityType = ActivityType.UserFollowed,
                TargetEntityId = notification.FollowingId,
                Metadata = JsonSerializer.Serialize(new { FollowingName = followingName }),
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
            };

            db.ActivityFeedItems.Add(feedItem);
            await db.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "Failed to create activity feed item for UserFollowed {FollowerId} -> {FollowingId}",
                notification.FollowerId, notification.FollowingId);
        }
    }
}
