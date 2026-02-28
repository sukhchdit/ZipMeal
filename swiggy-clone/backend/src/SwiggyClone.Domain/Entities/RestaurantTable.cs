using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents a physical dining table at a <see cref="Restaurant"/>.
/// Each table has a unique QR code for dine-in ordering and can be independently
/// deactivated via <see cref="IsActive"/> without deletion. The combination of
/// <see cref="RestaurantId"/> and <see cref="TableNumber"/> must be unique.
/// </summary>
public sealed class RestaurantTable
{
    /// <summary>
    /// Unique identifier for this table (UUID v7).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="Restaurant"/> this table belongs to.
    /// </summary>
    public Guid RestaurantId { get; set; }

    /// <summary>
    /// Display number or label for the table (max 20 characters), e.g., "T1", "A-12".
    /// Must be unique within the restaurant.
    /// </summary>
    public string TableNumber { get; set; } = string.Empty;

    /// <summary>
    /// Maximum seating capacity of the table. Defaults to 4.
    /// </summary>
    public int Capacity { get; set; } = 4;

    /// <summary>
    /// Floor or section of the restaurant where this table is located (max 50 characters),
    /// e.g., "Ground Floor", "Rooftop", "Private Dining".
    /// </summary>
    public string? FloorSection { get; set; }

    /// <summary>
    /// Encoded QR code data used by customers to scan and start a dine-in session (max 500 characters).
    /// Must be unique across all tables on the platform.
    /// </summary>
    public string QrCodeData { get; set; } = string.Empty;

    /// <summary>
    /// Current occupancy status of the table (Available, Occupied, Reserved, Maintenance).
    /// Stored as a SMALLINT in the database.
    /// </summary>
    public TableStatus Status { get; set; }

    /// <summary>
    /// Indicates whether this table is active and available for use. Defaults to true.
    /// Inactive tables are hidden from the dine-in flow.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Timestamp when this table record was created (UTC).
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when this table record was last updated (UTC).
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// The restaurant this table belongs to.
    /// </summary>
    public Restaurant Restaurant { get; set; } = null!;

    /// <summary>
    /// Dine-in sessions that have been held at this table.
    /// </summary>
    public ICollection<DineInSession> DineInSessions { get; set; } = [];
}
