namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents an optional add-on that customers can attach to a <see cref="MenuItem"/>
/// (e.g., Extra Cheese, Butter, Dressing). Each add-on has its own price and can be
/// independently toggled for availability. This is a simple entity without soft-delete support.
/// </summary>
public sealed class MenuItemAddon
{
    /// <summary>
    /// Unique identifier for this add-on (UUID v7).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="MenuItem"/> this add-on belongs to.
    /// </summary>
    public Guid MenuItemId { get; set; }

    /// <summary>
    /// Display name of the add-on (max 100 characters), e.g., "Extra Cheese", "Garlic Bread".
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Price of the add-on in paise (e.g., 4900 = INR 49.00). Defaults to 0 for free add-ons.
    /// </summary>
    public int Price { get; set; }

    /// <summary>
    /// Indicates whether this add-on is vegetarian. Defaults to true.
    /// </summary>
    public bool IsVeg { get; set; } = true;

    /// <summary>
    /// Indicates whether this add-on is currently available for selection. Defaults to true.
    /// </summary>
    public bool IsAvailable { get; set; } = true;

    /// <summary>
    /// Maximum quantity of this add-on that can be added to a single menu item. Defaults to 5.
    /// </summary>
    public int MaxQuantity { get; set; } = 5;

    /// <summary>
    /// Sort order for displaying add-ons within a menu item.
    /// Lower values appear first. Defaults to 0.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Timestamp when this add-on was created (UTC).
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// The menu item this add-on belongs to.
    /// </summary>
    public MenuItem MenuItem { get; set; } = null!;
}
