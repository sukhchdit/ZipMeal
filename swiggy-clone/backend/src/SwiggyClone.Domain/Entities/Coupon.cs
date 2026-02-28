using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents a discount coupon that customers can apply to orders.
/// Supports both percentage-based and flat-amount discounts, with configurable
/// usage limits, validity periods, and applicability to specific order types or restaurants.
/// All monetary amounts are stored in paise (smallest currency unit).
/// </summary>
public sealed class Coupon
{
    /// <summary>
    /// Unique identifier for this coupon (UUID v7).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Unique coupon code entered by the customer (max 20 characters), e.g., "WELCOME50", "FLAT100".
    /// Case-insensitive matching should be enforced at the application layer.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Display title of the coupon (max 200 characters), e.g., "50% off on your first order".
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the coupon terms and conditions.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Type of discount: Percentage or FlatAmount. Stored as a SMALLINT in the database.
    /// </summary>
    public DiscountType DiscountType { get; set; }

    /// <summary>
    /// Discount value in the unit determined by <see cref="DiscountType"/>.
    /// For Percentage: the percentage value (e.g., 50 = 50%).
    /// For FlatAmount: the amount in paise (e.g., 10000 = INR 100.00).
    /// </summary>
    public int DiscountValue { get; set; }

    /// <summary>
    /// Maximum discount amount in paise for percentage-based coupons.
    /// Null for flat-amount coupons or when there is no cap.
    /// </summary>
    public int? MaxDiscount { get; set; }

    /// <summary>
    /// Minimum order subtotal required (in paise) to apply this coupon. Defaults to 0.
    /// </summary>
    public int MinOrderAmount { get; set; }

    /// <summary>
    /// Start of the coupon validity period. The coupon cannot be used before this timestamp.
    /// </summary>
    public DateTimeOffset ValidFrom { get; set; }

    /// <summary>
    /// End of the coupon validity period. The coupon cannot be used after this timestamp.
    /// </summary>
    public DateTimeOffset ValidUntil { get; set; }

    /// <summary>
    /// Maximum total number of times this coupon can be redeemed across all users.
    /// Null for unlimited usage.
    /// </summary>
    public int? MaxUsages { get; set; }

    /// <summary>
    /// Maximum number of times a single user can redeem this coupon. Defaults to 1.
    /// </summary>
    public int MaxUsagesPerUser { get; set; } = 1;

    /// <summary>
    /// Current total number of times this coupon has been redeemed. Defaults to 0.
    /// Incremented atomically on successful order placement.
    /// </summary>
    public int CurrentUsages { get; set; }

    /// <summary>
    /// Array of <see cref="OrderType"/> values (as short) this coupon applies to.
    /// Stored as a PostgreSQL SMALLINT array. Empty array means applicable to all order types.
    /// </summary>
    public short[] ApplicableOrderTypes { get; set; } = [];

    /// <summary>
    /// Array of restaurant IDs this coupon is restricted to.
    /// Null means the coupon is valid at all restaurants.
    /// Stored as a PostgreSQL UUID array.
    /// </summary>
    public Guid[]? RestaurantIds { get; set; }

    /// <summary>
    /// Indicates whether this coupon is active and available for use. Defaults to true.
    /// Inactive coupons are hidden from the coupon listing.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Timestamp when this coupon was created (UTC).
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when this coupon was last updated (UTC).
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
}
