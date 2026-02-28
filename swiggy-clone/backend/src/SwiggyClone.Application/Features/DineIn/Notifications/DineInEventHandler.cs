using MediatR;
using Microsoft.Extensions.Logging;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared.Constants;

namespace SwiggyClone.Application.Features.DineIn.Notifications;

internal sealed class DineInEventHandler(
    IEventBus eventBus,
    IRealtimeNotifier realtimeNotifier,
    ILogger<DineInEventHandler> logger)
    : INotificationHandler<DineInSessionStartedNotification>,
      INotificationHandler<DineInMemberJoinedNotification>,
      INotificationHandler<DineInMemberLeftNotification>,
      INotificationHandler<DineInOrderPlacedNotification>,
      INotificationHandler<DineInOrderStatusChangedNotification>,
      INotificationHandler<DineInBillRequestedNotification>,
      INotificationHandler<DineInSessionEndedNotification>
{
    public async Task Handle(DineInSessionStartedNotification notification, CancellationToken ct)
    {
        try
        {
            await eventBus.PublishAsync(
                KafkaTopics.DineInSessionStarted,
                notification.SessionId.ToString(),
                new
                {
                    notification.SessionId,
                    notification.RestaurantId,
                    notification.TableNumber,
                    Timestamp = DateTimeOffset.UtcNow
                }, ct);

            await realtimeNotifier.NotifyDineInEventAsync(
                notification.SessionId,
                "SessionStarted",
                new { notification.TableNumber },
                ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to publish DineInSessionStarted for {SessionId}", notification.SessionId);
        }
    }

    public async Task Handle(DineInMemberJoinedNotification notification, CancellationToken ct)
    {
        try
        {
            await realtimeNotifier.NotifyDineInEventAsync(
                notification.SessionId,
                "MemberJoined",
                new { notification.UserId, notification.UserName },
                ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to publish DineInMemberJoined for {SessionId}", notification.SessionId);
        }
    }

    public async Task Handle(DineInMemberLeftNotification notification, CancellationToken ct)
    {
        try
        {
            await realtimeNotifier.NotifyDineInEventAsync(
                notification.SessionId,
                "MemberLeft",
                new { notification.UserId },
                ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to publish DineInMemberLeft for {SessionId}", notification.SessionId);
        }
    }

    public async Task Handle(DineInOrderPlacedNotification notification, CancellationToken ct)
    {
        try
        {
            await eventBus.PublishAsync(
                KafkaTopics.DineInOrderPlaced,
                notification.SessionId.ToString(),
                new
                {
                    notification.SessionId,
                    notification.OrderId,
                    notification.UserId,
                    notification.TotalAmount,
                    Timestamp = DateTimeOffset.UtcNow
                }, ct);

            await realtimeNotifier.NotifyDineInEventAsync(
                notification.SessionId,
                "OrderPlaced",
                new { notification.OrderId, notification.UserId, notification.TotalAmount },
                ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to publish DineInOrderPlaced for {SessionId}", notification.SessionId);
        }
    }

    public async Task Handle(DineInOrderStatusChangedNotification notification, CancellationToken ct)
    {
        try
        {
            await realtimeNotifier.NotifyDineInEventAsync(
                notification.SessionId,
                "OrderStatusChanged",
                new { notification.OrderId, notification.NewStatus },
                ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to publish DineInOrderStatusChanged for {SessionId}", notification.SessionId);
        }
    }

    public async Task Handle(DineInBillRequestedNotification notification, CancellationToken ct)
    {
        try
        {
            await eventBus.PublishAsync(
                KafkaTopics.DineInBillRequested,
                notification.SessionId.ToString(),
                new
                {
                    notification.SessionId,
                    notification.RestaurantId,
                    Timestamp = DateTimeOffset.UtcNow
                }, ct);

            await realtimeNotifier.NotifyDineInEventAsync(
                notification.SessionId,
                "BillRequested",
                new { notification.RestaurantId },
                ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to publish DineInBillRequested for {SessionId}", notification.SessionId);
        }
    }

    public async Task Handle(DineInSessionEndedNotification notification, CancellationToken ct)
    {
        try
        {
            await realtimeNotifier.NotifyDineInEventAsync(
                notification.SessionId,
                "SessionEnded",
                new { notification.TableNumber },
                ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to publish DineInSessionEnded for {SessionId}", notification.SessionId);
        }
    }
}
