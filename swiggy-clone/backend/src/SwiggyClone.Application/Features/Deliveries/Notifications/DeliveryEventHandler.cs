using MediatR;
using Microsoft.Extensions.Logging;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared.Constants;

namespace SwiggyClone.Application.Features.Deliveries.Notifications;

internal sealed class DeliveryEventHandler(
    IEventBus eventBus,
    IRealtimeNotifier realtimeNotifier,
    ILogger<DeliveryEventHandler> logger)
    : INotificationHandler<DeliveryAssignedNotification>,
      INotificationHandler<DeliveryStatusChangedNotification>,
      INotificationHandler<DeliveryLocationUpdatedNotification>
{
    public async Task Handle(DeliveryAssignedNotification notification, CancellationToken ct)
    {
        try
        {
            await eventBus.PublishAsync(
                KafkaTopics.DeliveryAssigned,
                notification.OrderId.ToString(),
                new
                {
                    notification.AssignmentId,
                    notification.OrderId,
                    notification.PartnerId,
                    notification.UserId,
                    Timestamp = DateTimeOffset.UtcNow
                }, ct);

            await realtimeNotifier.NotifyOrderStatusAsync(
                notification.UserId,
                notification.OrderId,
                "DeliveryAssigned",
                new { notification.PartnerId },
                ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to publish DeliveryAssigned event for {OrderId}", notification.OrderId);
        }
    }

    public async Task Handle(DeliveryStatusChangedNotification notification, CancellationToken ct)
    {
        try
        {
            await eventBus.PublishAsync(
                KafkaTopics.DeliveryAssigned,
                notification.OrderId.ToString(),
                new
                {
                    notification.AssignmentId,
                    notification.OrderId,
                    notification.UserId,
                    notification.NewStatus,
                    Timestamp = DateTimeOffset.UtcNow
                }, ct);

            await realtimeNotifier.NotifyOrderStatusAsync(
                notification.UserId,
                notification.OrderId,
                notification.NewStatus,
                new { notification.AssignmentId },
                ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to publish DeliveryStatusChanged event for {OrderId}", notification.OrderId);
        }
    }

    public async Task Handle(DeliveryLocationUpdatedNotification notification, CancellationToken ct)
    {
        try
        {
            await eventBus.PublishAsync(
                KafkaTopics.DeliveryLocationUpdated,
                notification.PartnerId.ToString(),
                new
                {
                    notification.OrderId,
                    notification.PartnerId,
                    notification.Latitude,
                    notification.Longitude,
                    notification.Heading,
                    notification.Speed,
                    Timestamp = DateTimeOffset.UtcNow
                }, ct);

            await realtimeNotifier.NotifyDeliveryLocationAsync(
                notification.OrderId,
                notification.Latitude,
                notification.Longitude,
                notification.Heading,
                ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to publish DeliveryLocationUpdated event for order {OrderId}", notification.OrderId);
        }
    }
}
