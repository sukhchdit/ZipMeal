namespace SwiggyClone.Application.Common.Interfaces;

/// <summary>
/// Abstraction for pushing real-time notifications to connected clients (SignalR).
/// </summary>
public interface IRealtimeNotifier
{
    Task NotifyOrderStatusAsync(Guid userId, Guid orderId, string status, object details, CancellationToken ct = default);
    Task NotifyDeliveryLocationAsync(Guid orderId, double latitude, double longitude, double? heading, CancellationToken ct = default);
    Task NotifyDineInEventAsync(Guid sessionId, string eventType, object details, CancellationToken ct = default);
}
