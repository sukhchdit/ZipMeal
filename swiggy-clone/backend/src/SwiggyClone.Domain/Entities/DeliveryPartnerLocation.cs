namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents the real-time location of a delivery partner.
/// Updated frequently as the partner moves, enabling live tracking on the customer's map.
/// Only the latest location is relevant; historical locations are not retained in this entity.
/// </summary>
public sealed class DeliveryPartnerLocation
{
    /// <summary>
    /// Unique identifier for this location record (UUID v7).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="User"/> (delivery partner) whose location this represents.
    /// </summary>
    public Guid PartnerId { get; set; }

    /// <summary>
    /// Current latitude coordinate of the delivery partner.
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Current longitude coordinate of the delivery partner.
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Compass heading in degrees (0-360) indicating the direction the partner is moving.
    /// Null when heading data is unavailable from the device.
    /// </summary>
    public double? Heading { get; set; }

    /// <summary>
    /// Current speed of the delivery partner in meters per second.
    /// Null when speed data is unavailable from the device.
    /// </summary>
    public double? Speed { get; set; }

    /// <summary>
    /// Indicates whether the delivery partner is currently online and available for assignments.
    /// Defaults to false.
    /// </summary>
    public bool IsOnline { get; set; }

    /// <summary>
    /// Timestamp when this location was last updated (UTC).
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// The delivery partner whose location this represents.
    /// </summary>
    public User Partner { get; set; } = null!;
}
