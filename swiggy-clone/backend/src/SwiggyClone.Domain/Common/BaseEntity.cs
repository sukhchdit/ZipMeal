namespace SwiggyClone.Domain.Common;

/// <summary>
/// Base entity with UUID v7 primary key, timestamps, and soft-delete support.
/// All domain entities must inherit from this class.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// UUID v7 primary key — time-sortable, index-friendly.
    /// </summary>
    public Guid Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }

    private readonly List<DomainEvent> _domainEvents = [];

    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Raises a domain event to be dispatched after the entity is persisted.
    /// </summary>
    public void RaiseDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// Marks the entity as soft-deleted.
    /// </summary>
    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
    }
}
