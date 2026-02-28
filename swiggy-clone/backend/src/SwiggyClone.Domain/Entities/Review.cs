namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents a customer review and rating for a restaurant after an order is completed.
/// Each order can have at most one review (enforced via a unique constraint on <see cref="OrderId"/>).
/// Supports anonymous reviews, restaurant replies, and optional delivery partner ratings.
/// This entity does not support soft-delete; visibility is controlled via <see cref="IsVisible"/>.
/// </summary>
public sealed class Review
{
    /// <summary>
    /// Unique identifier for this review (UUID v7).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="Order"/> this review is associated with.
    /// Must be unique — only one review per order is allowed.
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="User"/> who submitted this review.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="Restaurant"/> being reviewed.
    /// </summary>
    public Guid RestaurantId { get; set; }

    /// <summary>
    /// Restaurant rating on a scale of 1 to 5 (inclusive). Stored as a SMALLINT.
    /// </summary>
    public short Rating { get; set; }

    /// <summary>
    /// Optional written review text providing detailed feedback about the experience.
    /// </summary>
    public string? ReviewText { get; set; }

    /// <summary>
    /// Optional rating for the delivery experience on a scale of 1 to 5 (inclusive).
    /// Null for dine-in and takeaway orders. Stored as a SMALLINT.
    /// </summary>
    public short? DeliveryRating { get; set; }

    /// <summary>
    /// Indicates whether the review should be displayed without revealing the reviewer's identity.
    /// Defaults to false.
    /// </summary>
    public bool IsAnonymous { get; set; }

    /// <summary>
    /// Optional reply from the restaurant owner or manager to this review.
    /// </summary>
    public string? RestaurantReply { get; set; }

    /// <summary>
    /// Timestamp when the restaurant replied to this review. Null if no reply has been posted.
    /// </summary>
    public DateTimeOffset? RepliedAt { get; set; }

    /// <summary>
    /// Indicates whether this review is visible to other users. Defaults to true.
    /// Can be set to false by moderation to hide inappropriate content.
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Timestamp when this review was created (UTC).
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when this review was last updated (UTC).
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// The order this review is associated with.
    /// </summary>
    public Order Order { get; set; } = null!;

    /// <summary>
    /// The user who submitted this review.
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// The restaurant being reviewed.
    /// </summary>
    public Restaurant Restaurant { get; set; } = null!;

    /// <summary>
    /// Photos attached to this review.
    /// </summary>
    public ICollection<ReviewPhoto> Photos { get; set; } = [];
}
