using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents a notification sent to a <see cref="User"/> (e.g., order updates, promotions,
/// dine-in events, system alerts). Notifications are stored for in-app display and can be
/// marked as read by the user. The <see cref="Data"/> field holds additional context as JSON.
/// </summary>
public sealed class Notification
{
    /// <summary>
    /// Unique identifier for this notification (UUID v7).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="User"/> this notification is addressed to.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Short title or headline of the notification (max 200 characters).
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Full body text of the notification message.
    /// </summary>
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// Category of the notification (OrderUpdate, Promotion, DineIn, System).
    /// Stored as a SMALLINT in the database.
    /// </summary>
    public NotificationType Type { get; set; }

    /// <summary>
    /// Additional structured data associated with the notification, stored as a JSON string
    /// (JSONB in PostgreSQL). May include order IDs, deep-link URLs, or action parameters.
    /// </summary>
    public string? Data { get; set; }

    /// <summary>
    /// Indicates whether the user has read this notification. Defaults to false.
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Timestamp when the user read this notification. Null if the notification has not been read.
    /// </summary>
    public DateTimeOffset? ReadAt { get; set; }

    /// <summary>
    /// Timestamp when this notification was created and sent (UTC).
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// The user this notification is addressed to.
    /// </summary>
    public User User { get; set; } = null!;
}
