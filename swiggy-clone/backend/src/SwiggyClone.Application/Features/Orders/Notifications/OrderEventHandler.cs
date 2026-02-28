using MediatR;
using Microsoft.Extensions.Logging;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared.Constants;

namespace SwiggyClone.Application.Features.Orders.Notifications;

internal sealed class OrderEventHandler(
    IEventBus eventBus,
    IRealtimeNotifier realtimeNotifier,
    ILogger<OrderEventHandler> logger)
    : INotificationHandler<OrderPlacedNotification>,
      INotificationHandler<OrderStatusChangedNotification>,
      INotificationHandler<OrderCancelledNotification>,
      INotificationHandler<OrderScheduledNotification>
{
    public async Task Handle(OrderPlacedNotification notification, CancellationToken ct)
    {
        try
        {
            await eventBus.PublishAsync(
                KafkaTopics.OrderCreated,
                notification.OrderId.ToString(),
                new
                {
                    notification.OrderId,
                    notification.UserId,
                    notification.RestaurantId,
                    notification.OrderNumber,
                    notification.TotalAmount,
                    Timestamp = DateTimeOffset.UtcNow
                }, ct);

            await realtimeNotifier.NotifyOrderStatusAsync(
                notification.UserId,
                notification.OrderId,
                "Placed",
                new { notification.OrderNumber, notification.TotalAmount },
                ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to publish OrderPlaced event for {OrderId}", notification.OrderId);
        }
    }

    public async Task Handle(OrderStatusChangedNotification notification, CancellationToken ct)
    {
        try
        {
            var topic = notification.NewStatus switch
            {
                "Confirmed" => KafkaTopics.OrderConfirmed,
                "Preparing" => KafkaTopics.OrderPreparing,
                "ReadyForPickup" => KafkaTopics.OrderReady,
                "OutForDelivery" => KafkaTopics.OrderReady,
                "Delivered" => KafkaTopics.OrderDelivered,
                _ => KafkaTopics.OrderCreated
            };

            await eventBus.PublishAsync(
                topic,
                notification.OrderId.ToString(),
                new
                {
                    notification.OrderId,
                    notification.UserId,
                    notification.NewStatus,
                    notification.OrderNumber,
                    Timestamp = DateTimeOffset.UtcNow
                }, ct);

            await realtimeNotifier.NotifyOrderStatusAsync(
                notification.UserId,
                notification.OrderId,
                notification.NewStatus,
                new { notification.OrderNumber },
                ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to publish OrderStatusChanged event for {OrderId}", notification.OrderId);
        }
    }

    public async Task Handle(OrderScheduledNotification notification, CancellationToken ct)
    {
        try
        {
            await eventBus.PublishAsync(
                KafkaTopics.OrderCreated,
                notification.OrderId.ToString(),
                new
                {
                    notification.OrderId,
                    notification.UserId,
                    notification.OrderNumber,
                    notification.TotalAmount,
                    notification.ScheduledDeliveryTime,
                    Status = "Scheduled",
                    Timestamp = DateTimeOffset.UtcNow
                }, ct);

            await realtimeNotifier.NotifyOrderStatusAsync(
                notification.UserId,
                notification.OrderId,
                "Scheduled",
                new { notification.OrderNumber, notification.TotalAmount, notification.ScheduledDeliveryTime },
                ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to publish OrderScheduled event for {OrderId}", notification.OrderId);
        }
    }

    public async Task Handle(OrderCancelledNotification notification, CancellationToken ct)
    {
        try
        {
            await eventBus.PublishAsync(
                KafkaTopics.OrderCancelled,
                notification.OrderId.ToString(),
                new
                {
                    notification.OrderId,
                    notification.UserId,
                    notification.Reason,
                    notification.RefundInitiated,
                    Timestamp = DateTimeOffset.UtcNow
                }, ct);

            await realtimeNotifier.NotifyOrderStatusAsync(
                notification.UserId,
                notification.OrderId,
                "Cancelled",
                new { notification.Reason, notification.RefundInitiated },
                ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to publish OrderCancelled event for {OrderId}", notification.OrderId);
        }
    }
}
