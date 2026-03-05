using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents a participant in a <see cref="GroupOrder"/>.
/// The initiator (IsInitiator = true) creates the group order; other participants
/// join using the invite code. Each participant manages their own sub-cart in Redis.
/// </summary>
public sealed class GroupOrderParticipant
{
    /// <summary>
    /// Unique identifier for this participant record (UUID v7).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="GroupOrder"/> this participant belongs to.
    /// </summary>
    public Guid GroupOrderId { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="User"/> participating in the group order.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Whether this participant is the initiator (creator) of the group order.
    /// </summary>
    public bool IsInitiator { get; set; }

    /// <summary>
    /// Current status of the participant (Joined, Ready, Left).
    /// Stored as a SMALLINT in the database.
    /// </summary>
    public GroupOrderParticipantStatus Status { get; set; }

    /// <summary>
    /// Timestamp when the participant joined the group order (UTC).
    /// </summary>
    public DateTimeOffset JoinedAt { get; set; }

    /// <summary>
    /// Timestamp when the participant left the group order (UTC).
    /// Null if the participant has not left.
    /// </summary>
    public DateTimeOffset? LeftAt { get; set; }

    /// <summary>
    /// Number of items in this participant's sub-cart.
    /// Updated when cart changes are made.
    /// </summary>
    public int ItemCount { get; set; }

    /// <summary>
    /// Total amount of this participant's sub-cart items, in paise.
    /// Updated when cart changes are made.
    /// </summary>
    public int ItemsTotal { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// The group order this participant belongs to.
    /// </summary>
    public GroupOrder GroupOrder { get; set; } = null!;

    /// <summary>
    /// The user participating in the group order.
    /// </summary>
    public User User { get; set; } = null!;
}
