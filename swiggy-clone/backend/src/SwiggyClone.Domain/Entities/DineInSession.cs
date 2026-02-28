using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents an active or completed dine-in session at a restaurant table.
/// A session is initiated when a customer scans the table QR code and can include
/// multiple guests (via <see cref="DineInSessionMember"/>) and multiple orders.
/// The <see cref="SessionCode"/> is a short, human-friendly code for sharing with guests.
/// </summary>
public sealed class DineInSession
{
    /// <summary>
    /// Unique identifier for this dine-in session (UUID v7).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="Restaurant"/> where this session is taking place.
    /// </summary>
    public Guid RestaurantId { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="RestaurantTable"/> assigned to this session.
    /// </summary>
    public Guid TableId { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="User"/> who initiated this dine-in session (the host).
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Short, unique alphanumeric code (max 10 characters) that guests can use to join
    /// this session, e.g., "AB12CD".
    /// </summary>
    public string SessionCode { get; set; } = string.Empty;

    /// <summary>
    /// Current status of the session (Active, BillRequested, PaymentPending, Completed, Cancelled).
    /// Stored as a SMALLINT in the database.
    /// </summary>
    public DineInSessionStatus Status { get; set; }

    /// <summary>
    /// Number of guests at the table including the host. Defaults to 1.
    /// </summary>
    public int GuestCount { get; set; } = 1;

    /// <summary>
    /// Timestamp when the session was started (customer scanned QR code).
    /// </summary>
    public DateTimeOffset StartedAt { get; set; }

    /// <summary>
    /// Timestamp when the session ended (bill paid and table freed).
    /// Null while the session is still active.
    /// </summary>
    public DateTimeOffset? EndedAt { get; set; }

    /// <summary>
    /// Timestamp when this session record was created (UTC).
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when this session record was last updated (UTC).
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// The restaurant where this dine-in session is taking place.
    /// </summary>
    public Restaurant Restaurant { get; set; } = null!;

    /// <summary>
    /// The table assigned to this dine-in session.
    /// </summary>
    public RestaurantTable Table { get; set; } = null!;

    /// <summary>
    /// The customer who initiated this dine-in session (host).
    /// </summary>
    public User Customer { get; set; } = null!;

    /// <summary>
    /// Members (host and guests) participating in this dine-in session.
    /// </summary>
    public ICollection<DineInSessionMember> Members { get; set; } = [];

    /// <summary>
    /// Orders placed during this dine-in session.
    /// </summary>
    public ICollection<Order> Orders { get; set; } = [];
}
