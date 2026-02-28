namespace SwiggyClone.Application.Common.Interfaces;

/// <summary>
/// Abstraction for publishing events to a message broker (Kafka).
/// </summary>
public interface IEventBus
{
    Task PublishAsync<T>(string topic, string key, T message, CancellationToken ct = default);
}
