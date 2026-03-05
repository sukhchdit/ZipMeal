using MediatR;
using Microsoft.Extensions.Logging;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared.Constants;

namespace SwiggyClone.Application.Features.GroupOrders.Notifications;

internal sealed class GroupOrderEventHandler(
    IEventBus eventBus,
    IRealtimeNotifier realtimeNotifier,
    ILogger<GroupOrderEventHandler> logger)
    : INotificationHandler<GroupOrderCreatedNotification>,
      INotificationHandler<GroupOrderParticipantJoinedNotification>,
      INotificationHandler<GroupOrderParticipantLeftNotification>,
      INotificationHandler<GroupOrderParticipantReadyNotification>,
      INotificationHandler<GroupOrderCartUpdatedNotification>,
      INotificationHandler<GroupOrderFinalizedNotification>,
      INotificationHandler<GroupOrderCancelledNotification>,
      INotificationHandler<GroupOrderExpiredNotification>
{
    public async Task Handle(GroupOrderCreatedNotification notification, CancellationToken ct)
    {
        try
        {
            await eventBus.PublishAsync(
                KafkaTopics.GroupOrderCreated,
                notification.GroupOrderId.ToString(),
                new
                {
                    notification.GroupOrderId,
                    notification.RestaurantId,
                    notification.InitiatorUserId,
                    notification.InviteCode,
                    Timestamp = DateTimeOffset.UtcNow
                }, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to publish GroupOrderCreated for {GroupOrderId}", notification.GroupOrderId);
        }
    }

    public async Task Handle(GroupOrderParticipantJoinedNotification notification, CancellationToken ct)
    {
        try
        {
            await realtimeNotifier.NotifyGroupOrderEventAsync(
                notification.GroupOrderId,
                "ParticipantJoined",
                new { notification.UserId, notification.UserName },
                ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to push ParticipantJoined for {GroupOrderId}", notification.GroupOrderId);
        }
    }

    public async Task Handle(GroupOrderParticipantLeftNotification notification, CancellationToken ct)
    {
        try
        {
            await realtimeNotifier.NotifyGroupOrderEventAsync(
                notification.GroupOrderId,
                "ParticipantLeft",
                new { notification.UserId },
                ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to push ParticipantLeft for {GroupOrderId}", notification.GroupOrderId);
        }
    }

    public async Task Handle(GroupOrderParticipantReadyNotification notification, CancellationToken ct)
    {
        try
        {
            await realtimeNotifier.NotifyGroupOrderEventAsync(
                notification.GroupOrderId,
                "ParticipantReady",
                new { notification.UserId },
                ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to push ParticipantReady for {GroupOrderId}", notification.GroupOrderId);
        }
    }

    public async Task Handle(GroupOrderCartUpdatedNotification notification, CancellationToken ct)
    {
        try
        {
            await realtimeNotifier.NotifyGroupOrderEventAsync(
                notification.GroupOrderId,
                "CartUpdated",
                new { notification.UserId, notification.ItemCount, notification.ItemsTotal },
                ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to push CartUpdated for {GroupOrderId}", notification.GroupOrderId);
        }
    }

    public async Task Handle(GroupOrderFinalizedNotification notification, CancellationToken ct)
    {
        try
        {
            await eventBus.PublishAsync(
                KafkaTopics.GroupOrderFinalized,
                notification.GroupOrderId.ToString(),
                new
                {
                    notification.GroupOrderId,
                    notification.OrderId,
                    notification.TotalAmount,
                    Timestamp = DateTimeOffset.UtcNow
                }, ct);

            await realtimeNotifier.NotifyGroupOrderEventAsync(
                notification.GroupOrderId,
                "GroupOrderFinalized",
                new { notification.OrderId, notification.TotalAmount },
                ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to publish GroupOrderFinalized for {GroupOrderId}", notification.GroupOrderId);
        }
    }

    public async Task Handle(GroupOrderCancelledNotification notification, CancellationToken ct)
    {
        try
        {
            await eventBus.PublishAsync(
                KafkaTopics.GroupOrderCancelled,
                notification.GroupOrderId.ToString(),
                new
                {
                    notification.GroupOrderId,
                    Timestamp = DateTimeOffset.UtcNow
                }, ct);

            await realtimeNotifier.NotifyGroupOrderEventAsync(
                notification.GroupOrderId,
                "GroupOrderCancelled",
                new { },
                ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to publish GroupOrderCancelled for {GroupOrderId}", notification.GroupOrderId);
        }
    }

    public async Task Handle(GroupOrderExpiredNotification notification, CancellationToken ct)
    {
        try
        {
            await realtimeNotifier.NotifyGroupOrderEventAsync(
                notification.GroupOrderId,
                "GroupOrderExpired",
                new { },
                ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to push GroupOrderExpired for {GroupOrderId}", notification.GroupOrderId);
        }
    }
}
