using MediatR;

namespace SwiggyClone.Domain.Common;

/// <summary>
/// Base class for all domain events. Domain events are dispatched after
/// the aggregate root is persisted, ensuring eventual consistency.
/// Implements MediatR INotification for in-process event dispatching.
/// </summary>
public abstract class DomainEvent : INotification
{
    public Guid EventId { get; } = Guid.NewGuid();

    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;

    public string EventType => GetType().Name;
}
