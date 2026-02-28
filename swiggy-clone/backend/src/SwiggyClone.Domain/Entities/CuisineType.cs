namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents a cuisine type (e.g., Indian, Chinese, Italian) that can be associated
/// with one or more restaurants via the <see cref="RestaurantCuisine"/> junction entity.
/// This is a simple reference/lookup entity without soft-delete support.
/// </summary>
public sealed class CuisineType
{
    /// <summary>
    /// Unique identifier for this cuisine type (UUID v7).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Display name of the cuisine type (max 50 characters). Must be unique across all cuisine types.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// URL to an icon image representing this cuisine type (max 500 characters).
    /// </summary>
    public string? IconUrl { get; set; }

    /// <summary>
    /// Sort order for displaying cuisine types in the UI. Lower values appear first. Defaults to 0.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Indicates whether this cuisine type is active and visible to users. Defaults to true.
    /// </summary>
    public bool IsActive { get; set; } = true;

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// Junction entities linking this cuisine type to restaurants that serve it.
    /// </summary>
    public ICollection<RestaurantCuisine> RestaurantCuisines { get; set; } = [];
}
