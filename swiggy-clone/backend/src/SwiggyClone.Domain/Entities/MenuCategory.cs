namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents a menu category within a restaurant (e.g., Starters, Main Course, Desserts).
/// Categories are used to organize <see cref="MenuItem"/> entities for display and navigation.
/// This is a simple entity without soft-delete support; deactivation is handled via
/// <see cref="IsActive"/>.
/// </summary>
public sealed class MenuCategory
{
    /// <summary>
    /// Unique identifier for this menu category (UUID v7).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="Restaurant"/> this category belongs to.
    /// </summary>
    public Guid RestaurantId { get; set; }

    /// <summary>
    /// Display name of the category (max 100 characters).
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the category (max 500 characters).
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Sort order for displaying categories within a restaurant menu.
    /// Lower values appear first. Defaults to 0.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Indicates whether this category is active and visible on the menu. Defaults to true.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Timestamp when this category was created (UTC).
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when this category was last updated (UTC).
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// The restaurant this category belongs to.
    /// </summary>
    public Restaurant Restaurant { get; set; } = null!;

    /// <summary>
    /// Menu items that belong to this category.
    /// </summary>
    public ICollection<MenuItem> MenuItems { get; set; } = [];
}
