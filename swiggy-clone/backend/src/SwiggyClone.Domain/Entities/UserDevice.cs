using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents a device registered by a <see cref="User"/> for receiving push notifications.
/// The combination of <see cref="UserId"/> and <see cref="DeviceToken"/> must be unique
/// to prevent duplicate registrations for the same device.
/// </summary>
public sealed class UserDevice
{
    /// <summary>
    /// Unique identifier for this device registration (UUID v7).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="User"/> who owns this device.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Push notification token issued by the platform's notification service
    /// (FCM for Android/Web, APNs for iOS). Max 512 characters.
    /// </summary>
    public string DeviceToken { get; set; } = string.Empty;

    /// <summary>
    /// Platform of the device (Android, iOS, Web). Stored as a SMALLINT in the database.
    /// </summary>
    public DevicePlatform Platform { get; set; }

    /// <summary>
    /// Indicates whether this device registration is active. Defaults to true.
    /// Set to false when the token becomes invalid or the user logs out from this device.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Timestamp when this device was registered (UTC).
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when this device registration was last updated (UTC).
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// The user who owns this device.
    /// </summary>
    public User User { get; set; } = null!;
}
