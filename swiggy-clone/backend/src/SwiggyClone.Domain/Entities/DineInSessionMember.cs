namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents a participant in a <see cref="DineInSession"/>.
/// The host (role = 1) is the user who initiated the session; guests (role = 2)
/// join using the session code. Each member can place their own orders within the session.
/// </summary>
public sealed class DineInSessionMember
{
    /// <summary>
    /// Unique identifier for this session member record (UUID v7).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="DineInSession"/> this member belongs to.
    /// </summary>
    public Guid SessionId { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="User"/> who is participating in the session.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Role of the member in the session. Stored as a SMALLINT.
    /// 1 = Host (session creator), 2 = Guest (joined via session code). Defaults to 1.
    /// </summary>
    public short Role { get; set; } = 1;

    /// <summary>
    /// Timestamp when the member joined the dine-in session (UTC).
    /// </summary>
    public DateTimeOffset JoinedAt { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// The dine-in session this member is participating in.
    /// </summary>
    public DineInSession Session { get; set; } = null!;

    /// <summary>
    /// The user who is participating as a member in this session.
    /// </summary>
    public User User { get; set; } = null!;
}
