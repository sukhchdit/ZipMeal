namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents a record of a <see cref="Coupon"/> being redeemed by a <see cref="User"/>
/// on a specific <see cref="Order"/>. Used to enforce per-user and global usage limits,
/// and to track the actual discount amount applied. All monetary amounts are stored in paise.
/// </summary>
public sealed class CouponUsage
{
    /// <summary>
    /// Unique identifier for this coupon usage record (UUID v7).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="Coupon"/> that was redeemed.
    /// </summary>
    public Guid CouponId { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="User"/> who redeemed the coupon.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="Order"/> the coupon was applied to.
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// Actual discount amount applied to the order in paise.
    /// May differ from the coupon's face value due to max-discount caps or order subtotal limits.
    /// </summary>
    public int DiscountAmount { get; set; }

    /// <summary>
    /// Timestamp when the coupon was redeemed (UTC).
    /// </summary>
    public DateTimeOffset UsedAt { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// The coupon that was redeemed.
    /// </summary>
    public Coupon Coupon { get; set; } = null!;

    /// <summary>
    /// The user who redeemed the coupon.
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// The order the coupon was applied to.
    /// </summary>
    public Order Order { get; set; } = null!;
}
