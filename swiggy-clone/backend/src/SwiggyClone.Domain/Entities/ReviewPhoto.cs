namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents a photo attached to a <see cref="Review"/>.
/// Customers can upload multiple photos to accompany their review,
/// displayed in the order determined by <see cref="SortOrder"/>.
/// </summary>
public sealed class ReviewPhoto
{
    /// <summary>
    /// Unique identifier for this review photo (UUID v7).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="Review"/> this photo belongs to.
    /// </summary>
    public Guid ReviewId { get; set; }

    /// <summary>
    /// URL to the photo image stored in cloud storage (max 500 characters).
    /// </summary>
    public string PhotoUrl { get; set; } = string.Empty;

    /// <summary>
    /// Sort order for displaying photos within a review.
    /// Lower values appear first. Defaults to 0.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Timestamp when this photo was uploaded (UTC).
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// The review this photo belongs to.
    /// </summary>
    public Review Review { get; set; } = null!;
}
