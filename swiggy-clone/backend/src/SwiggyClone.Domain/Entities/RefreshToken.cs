namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents a hashed refresh token issued during authentication.
/// Tokens are never soft-deleted; revoked tokens retain their <see cref="RevokedAt"/> timestamp
/// for audit purposes and are eventually purged by a background job.
/// </summary>
public class RefreshToken
{
    /// <summary>
    /// UUID v7 primary key.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="User"/> who owns this token.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// SHA-256 (or similar) hash of the raw refresh token value.
    /// The raw token is never stored; only the hash is persisted for verification.
    /// </summary>
    public string TokenHash { get; set; } = default!;

    /// <summary>
    /// Optional device/browser fingerprint or user-agent string captured at token issuance.
    /// Used to display active sessions and detect suspicious logins.
    /// </summary>
    public string? DeviceInfo { get; set; }

    /// <summary>
    /// Absolute expiration timestamp after which the token is no longer valid,
    /// even if it has not been revoked.
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// Timestamp when the token was explicitly revoked (logout, password change, admin action).
    /// A <c>null</c> value indicates the token has not been revoked.
    /// </summary>
    public DateTimeOffset? RevokedAt { get; set; }

    /// <summary>
    /// Timestamp when the token was created/issued.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    // ── Navigation Properties ──────────────────────────────────────────

    /// <summary>
    /// The user who owns this refresh token.
    /// </summary>
    public User User { get; set; } = default!;
}
