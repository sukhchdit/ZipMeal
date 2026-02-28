using SwiggyClone.Domain.Common;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents a platform user (customer, restaurant owner, delivery partner, or admin).
/// This is the aggregate root for the User bounded context, owning addresses,
/// refresh tokens, devices, orders, reviews, and favorite restaurants.
/// </summary>
public class User : AuditableEntity, IAggregateRoot
{
    /// <summary>
    /// Primary phone number used for OTP-based authentication.
    /// Must be unique across all users. Maximum 15 characters (E.164 format).
    /// </summary>
    public string PhoneNumber { get; set; } = default!;

    /// <summary>
    /// Optional email address for notifications and account recovery.
    /// Must be unique when provided.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Full display name of the user.
    /// </summary>
    public string FullName { get; set; } = default!;

    /// <summary>
    /// URL pointing to the user's profile avatar image.
    /// </summary>
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// BCrypt hash of the user's password. Null for phone-only (OTP) registrations.
    /// </summary>
    public string? PasswordHash { get; set; }

    /// <summary>
    /// The role assigned to this user, determining their access level and capabilities.
    /// Defaults to <see cref="UserRole.Customer"/> for new sign-ups.
    /// </summary>
    public UserRole Role { get; set; } = UserRole.Customer;

    /// <summary>
    /// Indicates whether the user has completed phone/email verification.
    /// </summary>
    public bool IsVerified { get; set; }

    /// <summary>
    /// Indicates whether the user account is active. Inactive accounts cannot log in.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Timestamp of the user's most recent successful login, if any.
    /// </summary>
    public DateTimeOffset? LastLoginAt { get; set; }

    // ── Navigation Properties ──────────────────────────────────────────

    /// <summary>
    /// Saved delivery/billing addresses for the user.
    /// </summary>
    public ICollection<UserAddress> Addresses { get; set; } = [];

    /// <summary>
    /// Active and revoked refresh tokens issued to this user.
    /// </summary>
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];

    /// <summary>
    /// Orders placed by this user.
    /// </summary>
    public ICollection<Order> Orders { get; set; } = [];

    /// <summary>
    /// Reviews and ratings submitted by this user.
    /// </summary>
    public ICollection<Review> Reviews { get; set; } = [];

    /// <summary>
    /// Devices registered for push notifications.
    /// </summary>
    public ICollection<UserDevice> Devices { get; set; } = [];

    /// <summary>
    /// Restaurants the user has marked as favorites.
    /// </summary>
    public ICollection<UserFavorite> FavoriteRestaurants { get; set; } = [];
}
