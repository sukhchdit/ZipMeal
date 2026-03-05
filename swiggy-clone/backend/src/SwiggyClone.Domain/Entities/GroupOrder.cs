using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents a collaborative group ordering session where multiple participants
/// can add items to their own sub-carts before the initiator finalizes into a
/// single combined delivery order. Expires after 60 minutes if not finalized.
/// </summary>
public sealed class GroupOrder
{
    /// <summary>
    /// Unique identifier for this group order (UUID v7).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="Restaurant"/> this group order is placed at.
    /// All participants must add items from this restaurant only.
    /// </summary>
    public Guid RestaurantId { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="User"/> who created this group order (the initiator).
    /// Only the initiator can finalize or cancel the group order.
    /// </summary>
    public Guid InitiatorUserId { get; set; }

    /// <summary>
    /// Short, unique 6-character alphanumeric invite code that others use to join
    /// this group order, e.g., "A3K9X2".
    /// </summary>
    public string InviteCode { get; set; } = string.Empty;

    /// <summary>
    /// Current status of the group order (Active, Finalizing, OrderPlaced, Expired, Cancelled).
    /// Stored as a SMALLINT in the database.
    /// </summary>
    public GroupOrderStatus Status { get; set; }

    /// <summary>
    /// How the payment will be split among participants.
    /// Stored as a SMALLINT in the database.
    /// </summary>
    public PaymentSplitType PaymentSplitType { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="UserAddress"/> for delivery.
    /// Set by the initiator before finalizing.
    /// </summary>
    public Guid? DeliveryAddressId { get; set; }

    /// <summary>
    /// Optional special instructions for the entire group order (max 500 characters).
    /// </summary>
    public string? SpecialInstructions { get; set; }

    /// <summary>
    /// Timestamp when this group order expires if not finalized.
    /// Defaults to 60 minutes after creation.
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// Timestamp when the initiator finalized this group order.
    /// Null while the group order is still active.
    /// </summary>
    public DateTimeOffset? FinalizedAt { get; set; }

    /// <summary>
    /// Foreign key to the final <see cref="Order"/> created when this group order is finalized.
    /// Null until the group order is finalized.
    /// </summary>
    public Guid? OrderId { get; set; }

    /// <summary>
    /// Timestamp when this group order record was created (UTC).
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when this group order record was last updated (UTC).
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// The restaurant this group order is for.
    /// </summary>
    public Restaurant Restaurant { get; set; } = null!;

    /// <summary>
    /// The user who initiated this group order.
    /// </summary>
    public User InitiatorUser { get; set; } = null!;

    /// <summary>
    /// The delivery address for this group order.
    /// </summary>
    public UserAddress? DeliveryAddress { get; set; }

    /// <summary>
    /// The final combined order created when this group order is finalized.
    /// </summary>
    public Order? Order { get; set; }

    /// <summary>
    /// Participants (initiator and joiners) in this group order.
    /// </summary>
    public ICollection<GroupOrderParticipant> Participants { get; set; } = [];
}
