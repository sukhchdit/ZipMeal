namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents an add-on attached to a specific <see cref="OrderItem"/>.
/// Captures a snapshot of the add-on name and pricing at the time of ordering,
/// ensuring historical accuracy even if the add-on is later modified or deleted.
/// All monetary amounts are stored in paise (smallest currency unit).
/// </summary>
public sealed class OrderItemAddon
{
    /// <summary>
    /// Unique identifier for this order item add-on (UUID v7).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="OrderItem"/> this add-on is attached to.
    /// </summary>
    public Guid OrderItemId { get; set; }

    /// <summary>
    /// Foreign key to the original <see cref="MenuItemAddon"/> that was selected.
    /// </summary>
    public Guid AddonId { get; set; }

    /// <summary>
    /// Snapshot of the add-on name at the time of ordering (max 100 characters).
    /// Preserved for historical accuracy.
    /// </summary>
    public string AddonName { get; set; } = string.Empty;

    /// <summary>
    /// Number of units of this add-on. Defaults to 1.
    /// </summary>
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// Price per unit of this add-on at the time of ordering, in paise.
    /// </summary>
    public int Price { get; set; }

    /// <summary>
    /// Timestamp when this order item add-on was created (UTC).
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// The order item this add-on is attached to.
    /// </summary>
    public OrderItem OrderItem { get; set; } = null!;

    /// <summary>
    /// The original menu item add-on that was selected.
    /// </summary>
    public MenuItemAddon Addon { get; set; } = null!;
}
