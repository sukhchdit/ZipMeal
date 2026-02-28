using MediatR;
using Microsoft.Extensions.Logging;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared.Constants;

namespace SwiggyClone.Application.Features.Reviews.Notifications;

internal sealed class ReviewEventHandler(
    IEventBus eventBus,
    ILogger<ReviewEventHandler> logger)
    : INotificationHandler<ReviewSubmittedNotification>
{
    public async Task Handle(ReviewSubmittedNotification notification, CancellationToken ct)
    {
        try
        {
            await eventBus.PublishAsync(
                KafkaTopics.ReviewSubmitted,
                notification.ReviewId.ToString(),
                new
                {
                    notification.ReviewId,
                    notification.UserId,
                    notification.RestaurantId,
                    Timestamp = DateTimeOffset.UtcNow
                }, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to publish ReviewSubmitted event for {ReviewId}", notification.ReviewId);
        }
    }
}
