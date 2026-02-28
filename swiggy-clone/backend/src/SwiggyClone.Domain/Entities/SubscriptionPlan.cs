namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents a subscription plan (e.g., "ZipMeal Pro") that users can subscribe to
/// for benefits like free delivery, extra discounts, and no surge fees.
/// This entity does not use soft-delete; plans are toggled via IsActive.
/// </summary>
public sealed class SubscriptionPlan
{
    /// <summary>
    /// Unique identifier for this plan (UUID v7).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Display name of the plan (max 100 characters).
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the plan benefits and terms.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Price of the plan in paise (smallest currency unit).
    /// </summary>
    public int PricePaise { get; set; }

    /// <summary>
    /// Duration of the subscription in days (e.g., 30 for monthly).
    /// </summary>
    public int DurationDays { get; set; }

    /// <summary>
    /// Whether subscribers get free delivery on all orders.
    /// </summary>
    public bool FreeDelivery { get; set; }

    /// <summary>
    /// Extra discount percentage applied to orders (0–100).
    /// </summary>
    public int ExtraDiscountPercent { get; set; }

    /// <summary>
    /// Whether surge fee is waived for subscribers.
    /// </summary>
    public bool NoSurgeFee { get; set; }

    /// <summary>
    /// Whether this plan is currently available for new subscriptions.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Timestamp when this plan was created (UTC).
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when this plan was last updated (UTC).
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// All user subscriptions associated with this plan.
    /// </summary>
    public ICollection<UserSubscription> Subscriptions { get; set; } = [];
}
