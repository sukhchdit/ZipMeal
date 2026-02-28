namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents the operating hours for a restaurant on a specific day of the week.
/// Each restaurant should have at most one entry per day (enforced via a unique constraint
/// on <see cref="RestaurantId"/> and <see cref="DayOfWeek"/>).
/// This is a simple entity without soft-delete support.
/// </summary>
public sealed class RestaurantOperatingHours
{
    /// <summary>
    /// Unique identifier for this operating hours record (UUID v7).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="Restaurant"/> these hours belong to.
    /// </summary>
    public Guid RestaurantId { get; set; }

    /// <summary>
    /// Day of the week as a short integer (0 = Sunday, 1 = Monday, ..., 6 = Saturday).
    /// Maps to <see cref="System.DayOfWeek"/>.
    /// </summary>
    public short DayOfWeek { get; set; }

    /// <summary>
    /// Time the restaurant opens on this day. Null if the restaurant is closed.
    /// </summary>
    public TimeOnly? OpenTime { get; set; }

    /// <summary>
    /// Time the restaurant closes on this day. Null if the restaurant is closed.
    /// </summary>
    public TimeOnly? CloseTime { get; set; }

    /// <summary>
    /// Indicates whether the restaurant is closed on this day. Defaults to false.
    /// When true, <see cref="OpenTime"/> and <see cref="CloseTime"/> are ignored.
    /// </summary>
    public bool IsClosed { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// The restaurant these operating hours belong to.
    /// </summary>
    public Restaurant Restaurant { get; set; } = null!;
}
