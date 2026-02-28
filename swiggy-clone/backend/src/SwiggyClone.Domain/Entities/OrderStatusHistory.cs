using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents a single status transition in the lifecycle of an <see cref="Order"/>.
/// Each entry records what status the order moved to, when the change occurred,
/// and optionally who triggered it and any notes. Provides a complete audit trail
/// for order status changes.
/// </summary>
public sealed class OrderStatusHistory
{
    /// <summary>
    /// Unique identifier for this status history entry (UUID v7).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="Order"/> this status change belongs to.
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// The order status that was transitioned to. Stored as a SMALLINT in the database.
    /// </summary>
    public OrderStatus Status { get; set; }

    /// <summary>
    /// Optional notes or comments about this status change (max 500 characters),
    /// e.g., "Customer requested cancellation", "Delivery partner reassigned".
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="User"/> who triggered this status change.
    /// Null for system-initiated transitions (e.g., auto-timeout cancellation).
    /// </summary>
    public Guid? ChangedBy { get; set; }

    /// <summary>
    /// Timestamp when this status change occurred (UTC).
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// The order this status history entry belongs to.
    /// </summary>
    public Order Order { get; set; } = null!;

    /// <summary>
    /// The user who triggered this status change, if any.
    /// </summary>
    public User? ChangedByUser { get; set; }
}
