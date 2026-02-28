using SwiggyClone.Domain.Common;

namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents an individual food or beverage item on a restaurant's menu.
/// Prices are stored in paise (smallest currency unit) to avoid floating-point precision issues.
/// Supports soft-delete via <see cref="BaseEntity"/> for safe removal without data loss.
/// </summary>
public sealed class MenuItem : BaseEntity
{
    /// <summary>
    /// Foreign key to the <see cref="MenuCategory"/> this item belongs to.
    /// </summary>
    public Guid CategoryId { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="Restaurant"/> this item belongs to.
    /// Denormalized for efficient querying without joining through categories.
    /// </summary>
    public Guid RestaurantId { get; set; }

    /// <summary>
    /// Display name of the menu item (max 200 characters).
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the menu item including ingredients and preparation style.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Base price of the item in paise (e.g., 29900 = INR 299.00).
    /// </summary>
    public int Price { get; set; }

    /// <summary>
    /// Discounted price in paise, if a promotion is active. Null when no discount applies.
    /// </summary>
    public int? DiscountedPrice { get; set; }

    /// <summary>
    /// URL to the item's display image (max 500 characters).
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Indicates whether the item is vegetarian. Defaults to true.
    /// </summary>
    public bool IsVeg { get; set; } = true;

    /// <summary>
    /// Indicates whether the item is currently available for ordering. Defaults to true.
    /// </summary>
    public bool IsAvailable { get; set; } = true;

    /// <summary>
    /// Indicates whether the item is marked as a bestseller. Defaults to false.
    /// </summary>
    public bool IsBestseller { get; set; }

    /// <summary>
    /// Estimated preparation time in minutes. Defaults to 15.
    /// </summary>
    public int PreparationTimeMin { get; set; } = 15;

    /// <summary>
    /// Sort order for displaying items within their category.
    /// Lower values appear first. Defaults to 0.
    /// </summary>
    public int SortOrder { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// The category this menu item belongs to.
    /// </summary>
    public MenuCategory Category { get; set; } = null!;

    /// <summary>
    /// The restaurant this menu item belongs to.
    /// </summary>
    public Restaurant Restaurant { get; set; } = null!;

    /// <summary>
    /// Size or option variants of this menu item (e.g., Small, Medium, Large).
    /// </summary>
    public ICollection<MenuItemVariant> Variants { get; set; } = [];

    /// <summary>
    /// Optional add-on items that can accompany this menu item (e.g., Extra Cheese, Dressing).
    /// </summary>
    public ICollection<MenuItemAddon> Addons { get; set; } = [];
}
