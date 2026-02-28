using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents a user's subscription to a plan. Immutable financial record —
/// does not use soft-delete. Status transitions: Active → Cancelled / Expired.
/// </summary>
public sealed class UserSubscription
{
    /// <summary>
    /// Unique identifier for this subscription record (UUID v7).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the subscribing user.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Foreign key to the subscription plan.
    /// </summary>
    public Guid PlanId { get; set; }

    /// <summary>
    /// When the subscription started.
    /// </summary>
    public DateTimeOffset StartDate { get; set; }

    /// <summary>
    /// When the subscription ends (StartDate + Plan.DurationDays).
    /// </summary>
    public DateTimeOffset EndDate { get; set; }

    /// <summary>
    /// Current status of the subscription.
    /// </summary>
    public SubscriptionStatus Status { get; set; }

    /// <summary>
    /// Amount paid in paise at the time of subscription.
    /// </summary>
    public int PaidAmountPaise { get; set; }

    /// <summary>
    /// Timestamp when this record was created (UTC).
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// The user who owns this subscription.
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// The plan this subscription is for.
    /// </summary>
    public SubscriptionPlan Plan { get; set; } = null!;
}
