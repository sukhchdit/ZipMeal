namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Junction entity representing a user's favorited menu item (dish).
/// Uses a composite primary key of (<see cref="UserId"/>, <see cref="MenuItemId"/>)
/// with no surrogate Guid identifier. This entity has no base class.
/// </summary>
public sealed class UserFavoriteItem
{
    /// <summary>
    /// Foreign key to the <see cref="User"/> who favorited the menu item.
    /// Part of the composite primary key.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="MenuItem"/> that was favorited.
    /// Part of the composite primary key.
    /// </summary>
    public Guid MenuItemId { get; set; }

    /// <summary>
    /// Timestamp when the user favorited this menu item (UTC).
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// The user who favorited the menu item.
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// The menu item that was favorited.
    /// </summary>
    public MenuItem MenuItem { get; set; } = null!;
}
