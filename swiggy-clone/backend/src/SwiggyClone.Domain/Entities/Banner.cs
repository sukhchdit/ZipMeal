using SwiggyClone.Domain.Common;

namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents a promotional banner displayed on the home feed.
/// Banners have a validity window, display ordering, and optional deep link for navigation.
/// All banners are admin-managed and can be toggled active/inactive.
/// </summary>
public sealed class Banner : BaseEntity
{
    /// <summary>
    /// Admin-facing title for the banner (max 200 characters).
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// CDN or storage URL for the banner image (max 500 characters).
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Optional deep link for navigation when the banner is tapped (max 500 characters).
    /// Can be a relative route path (e.g., "/restaurant/abc-123") or null for non-interactive banners.
    /// </summary>
    public string? DeepLink { get; set; }

    /// <summary>
    /// Sort priority for display ordering. Lower values appear first.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Whether this banner is active and eligible for display. Defaults to true.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Start of the visibility window. The banner is not shown before this timestamp.
    /// </summary>
    public DateTimeOffset ValidFrom { get; set; }

    /// <summary>
    /// End of the visibility window. The banner is not shown after this timestamp.
    /// </summary>
    public DateTimeOffset ValidUntil { get; set; }

    /// <summary>
    /// Creates a new banner with a UUID v7 identifier.
    /// </summary>
    public static Banner Create(
        string title,
        string imageUrl,
        string? deepLink,
        int displayOrder,
        DateTimeOffset validFrom,
        DateTimeOffset validUntil)
    {
        var now = DateTimeOffset.UtcNow;
        return new Banner
        {
            Id = Guid.CreateVersion7(),
            Title = title,
            ImageUrl = imageUrl,
            DeepLink = deepLink,
            DisplayOrder = displayOrder,
            IsActive = true,
            ValidFrom = validFrom,
            ValidUntil = validUntil,
            CreatedAt = now,
            UpdatedAt = now,
        };
    }

    /// <summary>
    /// Updates the banner's editable fields.
    /// </summary>
    public void Update(
        string title,
        string imageUrl,
        string? deepLink,
        int displayOrder,
        DateTimeOffset validFrom,
        DateTimeOffset validUntil)
    {
        Title = title;
        ImageUrl = imageUrl;
        DeepLink = deepLink;
        DisplayOrder = displayOrder;
        ValidFrom = validFrom;
        ValidUntil = validUntil;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Toggles the active status of this banner.
    /// </summary>
    public void ToggleActive()
    {
        IsActive = !IsActive;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
