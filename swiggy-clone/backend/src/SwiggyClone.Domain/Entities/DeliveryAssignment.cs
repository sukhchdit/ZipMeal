using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents the assignment of a delivery partner to an <see cref="Order"/>.
/// Tracks the full delivery lifecycle from assignment through acceptance, pickup, and delivery.
/// Includes real-time location tracking and earnings calculation for the delivery partner.
/// </summary>
public sealed class DeliveryAssignment
{
    /// <summary>
    /// Unique identifier for this delivery assignment (UUID v7).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="Order"/> being delivered.
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="User"/> (delivery partner) assigned to this delivery.
    /// </summary>
    public Guid PartnerId { get; set; }

    /// <summary>
    /// Current status of the delivery (Assigned, Accepted, PickedUp, EnRoute, Delivered, Cancelled).
    /// Stored as a SMALLINT in the database.
    /// </summary>
    public DeliveryStatus Status { get; set; }

    /// <summary>
    /// Timestamp when the delivery partner was assigned to this order.
    /// </summary>
    public DateTimeOffset AssignedAt { get; set; }

    /// <summary>
    /// Timestamp when the delivery partner accepted the assignment.
    /// Null if not yet accepted or if the assignment was cancelled/reassigned.
    /// </summary>
    public DateTimeOffset? AcceptedAt { get; set; }

    /// <summary>
    /// Timestamp when the delivery partner picked up the order from the restaurant.
    /// Null until pickup is confirmed.
    /// </summary>
    public DateTimeOffset? PickedUpAt { get; set; }

    /// <summary>
    /// Timestamp when the order was delivered to the customer.
    /// Null until delivery is confirmed.
    /// </summary>
    public DateTimeOffset? DeliveredAt { get; set; }

    /// <summary>
    /// Current latitude coordinate of the delivery partner for real-time tracking.
    /// Null when location data is unavailable.
    /// </summary>
    public double? CurrentLatitude { get; set; }

    /// <summary>
    /// Current longitude coordinate of the delivery partner for real-time tracking.
    /// Null when location data is unavailable.
    /// </summary>
    public double? CurrentLongitude { get; set; }

    /// <summary>
    /// Total distance of the delivery route in kilometers. Precision: 6 digits total, 2 decimal places.
    /// Null until the route is calculated.
    /// </summary>
    public decimal? DistanceKm { get; set; }

    /// <summary>
    /// Earnings for the delivery partner for this assignment, in paise. Defaults to 0.
    /// Calculated based on distance, time, and platform rates.
    /// </summary>
    public int Earnings { get; set; }

    /// <summary>
    /// Timestamp when this delivery assignment record was created (UTC).
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when this delivery assignment record was last updated (UTC).
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// The order being delivered.
    /// </summary>
    public Order Order { get; set; } = null!;

    /// <summary>
    /// The delivery partner assigned to this delivery.
    /// </summary>
    public User Partner { get; set; } = null!;
}
