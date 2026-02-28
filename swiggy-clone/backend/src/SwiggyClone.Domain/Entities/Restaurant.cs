using SwiggyClone.Domain.Common;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents a restaurant registered on the platform.
/// This is an aggregate root that encapsulates menu categories, menu items,
/// operating hours, and other restaurant-specific child entities.
/// Supports soft-delete via <see cref="BaseEntity"/>.
/// </summary>
public sealed class Restaurant : BaseEntity, IAggregateRoot
{
    /// <summary>
    /// Foreign key to the <see cref="User"/> who owns this restaurant.
    /// </summary>
    public Guid OwnerId { get; set; }

    /// <summary>
    /// Display name of the restaurant (max 200 characters).
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// URL-friendly unique slug derived from the restaurant name (max 200 characters).
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the restaurant, its specialties, and ambiance.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Comma-separated list of cuisine types the restaurant serves (max 500 characters).
    /// </summary>
    public string? CuisineTypes { get; set; }

    /// <summary>
    /// Contact phone number of the restaurant (max 15 characters, E.164 format).
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Contact email address of the restaurant (max 255 characters).
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// URL to the restaurant logo image (max 500 characters).
    /// </summary>
    public string? LogoUrl { get; set; }

    /// <summary>
    /// URL to the restaurant banner/cover image (max 500 characters).
    /// </summary>
    public string? BannerUrl { get; set; }

    /// <summary>
    /// Primary address line (street, building) of the restaurant (max 255 characters).
    /// </summary>
    public string? AddressLine1 { get; set; }

    /// <summary>
    /// Secondary address line (floor, landmark) of the restaurant (max 255 characters).
    /// </summary>
    public string? AddressLine2 { get; set; }

    /// <summary>
    /// City where the restaurant is located (max 100 characters).
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// State or province where the restaurant is located (max 100 characters).
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Postal/ZIP code of the restaurant address (max 10 characters).
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// Geographic latitude coordinate for map positioning and distance calculations.
    /// </summary>
    public double? Latitude { get; set; }

    /// <summary>
    /// Geographic longitude coordinate for map positioning and distance calculations.
    /// </summary>
    public double? Longitude { get; set; }

    /// <summary>
    /// Weighted average rating of the restaurant on a scale of 0.0 to 5.0.
    /// Precision: 2 digits total, 1 decimal place. Defaults to 0.0.
    /// </summary>
    public decimal AverageRating { get; set; }

    /// <summary>
    /// Total number of ratings received by the restaurant. Defaults to 0.
    /// </summary>
    public int TotalRatings { get; set; }

    /// <summary>
    /// Estimated average delivery time in minutes.
    /// </summary>
    public int? AvgDeliveryTimeMin { get; set; }

    /// <summary>
    /// Average cost for two people in paise (smallest currency unit). Defaults to 0.
    /// </summary>
    public int? AvgCostForTwo { get; set; }

    /// <summary>
    /// Indicates whether the restaurant serves only vegetarian food. Defaults to false.
    /// </summary>
    public bool IsVegOnly { get; set; }

    /// <summary>
    /// Indicates whether the restaurant is currently accepting orders. Defaults to true.
    /// </summary>
    public bool IsAcceptingOrders { get; set; } = true;

    /// <summary>
    /// Indicates whether dine-in table booking is enabled for this restaurant. Defaults to false.
    /// </summary>
    public bool IsDineInEnabled { get; set; }

    /// <summary>
    /// Platform commission rate as a percentage (e.g., 15.00 = 15%).
    /// Precision: 5 digits total, 2 decimal places. Defaults to 15.00.
    /// </summary>
    public decimal CommissionRate { get; set; } = 15.00m;

    /// <summary>
    /// FSSAI (Food Safety and Standards Authority of India) license number (max 20 characters).
    /// </summary>
    public string? FssaiLicense { get; set; }

    /// <summary>
    /// GST (Goods and Services Tax) registration number (max 20 characters).
    /// </summary>
    public string? GstNumber { get; set; }

    /// <summary>
    /// Current operational status of the restaurant (Pending, Approved, Suspended, Rejected).
    /// Stored as a SMALLINT in the database. Defaults to <see cref="RestaurantStatus.Pending"/>.
    /// </summary>
    public RestaurantStatus Status { get; set; } = RestaurantStatus.Pending;

    /// <summary>
    /// Optional reason for rejection or suspension by an admin (max 500 characters).
    /// Cleared when the restaurant is approved or reactivated.
    /// </summary>
    public string? StatusReason { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// The user who owns and manages this restaurant.
    /// </summary>
    public User Owner { get; set; } = null!;

    /// <summary>
    /// Weekly operating hours defining when the restaurant is open on each day.
    /// </summary>
    public ICollection<RestaurantOperatingHours> OperatingHours { get; set; } = [];

    /// <summary>
    /// Menu categories used to organize menu items (e.g., Starters, Main Course).
    /// </summary>
    public ICollection<MenuCategory> MenuCategories { get; set; } = [];

    /// <summary>
    /// All menu items offered by this restaurant across all categories.
    /// </summary>
    public ICollection<MenuItem> MenuItems { get; set; } = [];

    /// <summary>
    /// Orders placed at this restaurant.
    /// </summary>
    public ICollection<Order> Orders { get; set; } = [];

    /// <summary>
    /// Dine-in tables available at this restaurant.
    /// </summary>
    public ICollection<RestaurantTable> Tables { get; set; } = [];

    /// <summary>
    /// Customer reviews and ratings for this restaurant.
    /// </summary>
    public ICollection<Review> Reviews { get; set; } = [];

    /// <summary>
    /// Junction entities linking this restaurant to its cuisine types.
    /// </summary>
    public ICollection<RestaurantCuisine> RestaurantCuisines { get; set; } = [];
}
