using Microsoft.AspNetCore.SignalR;
using SwiggyClone.Api.Hubs;
using SwiggyClone.Application.Common.Interfaces;

namespace SwiggyClone.Api.Services;

internal sealed class SignalRRealtimeNotifier(
    IHubContext<OrderTrackingHub> orderTrackingHub,
    IHubContext<DineInHub> dineInHub,
    ILogger<SignalRRealtimeNotifier> logger) : IRealtimeNotifier
{
    public async Task NotifyOrderStatusAsync(
        Guid userId, Guid orderId, string status, object details, CancellationToken ct)
    {
        var payload = new { orderId, status, details, timestamp = DateTimeOffset.UtcNow };

        await Task.WhenAll(
            orderTrackingHub.Clients.Group($"order-{orderId}")
                .SendAsync("OrderStatusChanged", payload, ct),
            orderTrackingHub.Clients.Group($"user-{userId}")
                .SendAsync("OrderStatusChanged", payload, ct));

        logger.LogDebug("Pushed OrderStatusChanged to order-{OrderId} and user-{UserId}", orderId, userId);
    }

    public async Task NotifyDeliveryLocationAsync(
        Guid orderId, double latitude, double longitude, double? heading, CancellationToken ct)
    {
        var payload = new { orderId, latitude, longitude, heading, timestamp = DateTimeOffset.UtcNow };

        await orderTrackingHub.Clients.Group($"order-{orderId}")
            .SendAsync("DeliveryLocationUpdated", payload, ct);
    }

    public async Task NotifyDineInEventAsync(
        Guid sessionId, string eventType, object details, CancellationToken ct)
    {
        var payload = new { sessionId, eventType, details, timestamp = DateTimeOffset.UtcNow };

        await dineInHub.Clients.Group($"dinein-{sessionId}")
            .SendAsync("DineInEvent", payload, ct);

        logger.LogDebug("Pushed DineInEvent {EventType} to dinein-{SessionId}", eventType, sessionId);
    }
}
