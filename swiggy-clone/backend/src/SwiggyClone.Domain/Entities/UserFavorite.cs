namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Junction entity representing a user's favorited restaurant.
/// Uses a composite primary key of (<see cref="UserId"/>, <see cref="RestaurantId"/>)
/// with no surrogate Guid identifier. This entity has no base class.
/// </summary>
public sealed class UserFavorite
{
    /// <summary>
    /// Foreign key to the <see cref="User"/> who favorited the restaurant.
    /// Part of the composite primary key.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="Restaurant"/> that was favorited.
    /// Part of the composite primary key.
    /// </summary>
    public Guid RestaurantId { get; set; }

    /// <summary>
    /// Timestamp when the user favorited this restaurant (UTC).
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// The user who favorited the restaurant.
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// The restaurant that was favorited.
    /// </summary>
    public Restaurant Restaurant { get; set; } = null!;
}
