namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents a size or option variant of a <see cref="MenuItem"/> (e.g., Small, Medium, Large).
/// Each variant can adjust the base price of the parent menu item and can be independently
/// toggled for availability. This is a simple entity without soft-delete support.
/// </summary>
public sealed class MenuItemVariant
{
    /// <summary>
    /// Unique identifier for this variant (UUID v7).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="MenuItem"/> this variant belongs to.
    /// </summary>
    public Guid MenuItemId { get; set; }

    /// <summary>
    /// Display name of the variant (max 100 characters), e.g., "Regular", "Large", "Family Pack".
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Price adjustment in paise relative to the base item price.
    /// Positive values increase the price; negative values decrease it. Defaults to 0.
    /// </summary>
    public int PriceAdjustment { get; set; }

    /// <summary>
    /// Indicates whether this variant is selected by default when the item is added to cart.
    /// Defaults to false.
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// Indicates whether this variant is currently available for selection. Defaults to true.
    /// </summary>
    public bool IsAvailable { get; set; } = true;

    /// <summary>
    /// Sort order for displaying variants within a menu item.
    /// Lower values appear first. Defaults to 0.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Timestamp when this variant was created (UTC).
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// The menu item this variant belongs to.
    /// </summary>
    public MenuItem MenuItem { get; set; } = null!;
}
