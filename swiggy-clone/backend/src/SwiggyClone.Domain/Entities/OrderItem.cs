using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents a single line item within an <see cref="Order"/>.
/// Captures a snapshot of the menu item name and pricing at the time of ordering,
/// ensuring historical accuracy even if the menu item is later modified or deleted.
/// All monetary amounts are stored in paise (smallest currency unit).
/// </summary>
public sealed class OrderItem
{
    /// <summary>
    /// Unique identifier for this order item (UUID v7).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="Order"/> this item belongs to.
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="MenuItem"/> that was ordered.
    /// </summary>
    public Guid MenuItemId { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="MenuItemVariant"/> selected for this item, if any.
    /// Null when the item has no variant or the default variant was used.
    /// </summary>
    public Guid? VariantId { get; set; }

    /// <summary>
    /// Snapshot of the menu item name at the time of ordering (max 200 characters).
    /// Preserved for historical accuracy.
    /// </summary>
    public string ItemName { get; set; } = string.Empty;

    /// <summary>
    /// Number of units ordered. Must be greater than zero.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Price per unit at the time of ordering, in paise.
    /// </summary>
    public int UnitPrice { get; set; }

    /// <summary>
    /// Total price for this line item (UnitPrice x Quantity + add-ons), in paise.
    /// </summary>
    public int TotalPrice { get; set; }

    /// <summary>
    /// Optional special instructions for this specific item (max 500 characters),
    /// e.g., "No onions", "Extra spicy".
    /// </summary>
    public string? SpecialInstructions { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="GroupOrderParticipant"/> who added this item.
    /// Enables per-participant attribution for group delivery orders. Null for non-group orders.
    /// </summary>
    public Guid? GroupOrderParticipantId { get; set; }

    /// <summary>
    /// Current preparation status of this item, primarily used for dine-in order tracking.
    /// Stored as a SMALLINT in the database. Defaults to <see cref="OrderStatus.Placed"/>.
    /// </summary>
    public OrderStatus Status { get; set; }

    /// <summary>
    /// Timestamp when this order item was created (UTC).
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// The order this item belongs to.
    /// </summary>
    public Order Order { get; set; } = null!;

    /// <summary>
    /// The menu item that was ordered.
    /// </summary>
    public MenuItem MenuItem { get; set; } = null!;

    /// <summary>
    /// The variant selected for this item, if any.
    /// </summary>
    public MenuItemVariant? Variant { get; set; }

    /// <summary>
    /// The group order participant who added this item, if applicable.
    /// </summary>
    public GroupOrderParticipant? GroupOrderParticipant { get; set; }

    /// <summary>
    /// Add-ons attached to this order item.
    /// </summary>
    public ICollection<OrderItemAddon> Addons { get; set; } = [];
}
