namespace SwiggyClone.Domain.Common;

/// <summary>
/// Marker interface for aggregate roots.
/// Only aggregate roots should be persisted directly via repositories.
/// Child entities are persisted through their aggregate root.
/// </summary>
public interface IAggregateRoot
{
}
