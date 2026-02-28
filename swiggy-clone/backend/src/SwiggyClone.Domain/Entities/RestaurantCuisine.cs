namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Junction entity linking a <see cref="Restaurant"/> to a <see cref="CuisineType"/>.
/// Uses a composite primary key of (<see cref="RestaurantId"/>, <see cref="CuisineId"/>)
/// with no surrogate Guid identifier. This entity has no base class.
/// </summary>
public sealed class RestaurantCuisine
{
    /// <summary>
    /// Foreign key to the <see cref="Restaurant"/>.
    /// Part of the composite primary key.
    /// </summary>
    public Guid RestaurantId { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="CuisineType"/>.
    /// Part of the composite primary key.
    /// </summary>
    public Guid CuisineId { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// The restaurant associated with this cuisine type mapping.
    /// </summary>
    public Restaurant Restaurant { get; set; } = null!;

    /// <summary>
    /// The cuisine type associated with this restaurant mapping.
    /// </summary>
    public CuisineType CuisineType { get; set; } = null!;
}
