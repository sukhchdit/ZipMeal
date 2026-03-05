namespace SwiggyClone.Application.Common.Interfaces;

/// <summary>
/// Abstraction for pushing real-time notifications to connected clients (SignalR).
/// </summary>
public interface IRealtimeNotifier
{
    Task NotifyOrderStatusAsync(Guid userId, Guid orderId, string status, object details, CancellationToken ct = default);
    Task NotifyDeliveryLocationAsync(Guid orderId, double latitude, double longitude, double? heading, CancellationToken ct = default);
    Task NotifyDineInEventAsync(Guid sessionId, string eventType, object details, CancellationToken ct = default);
    Task NotifyChatMessageAsync(Guid ticketId, Guid recipientId, object messageDto, CancellationToken ct = default);
    Task NotifyChatTypingAsync(Guid ticketId, Guid userId, bool isTyping, CancellationToken ct = default);
    Task NotifyGroupOrderEventAsync(Guid groupOrderId, string eventType, object details, CancellationToken ct = default);
    Task NotifyDisputeEventAsync(Guid userId, Guid disputeId, string eventType, object details, CancellationToken ct = default);
}
