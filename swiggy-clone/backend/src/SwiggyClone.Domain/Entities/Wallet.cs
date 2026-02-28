namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents a user's wallet holding a monetary balance in paise.
/// Each user has at most one wallet (unique on UserId).
/// This entity does not support soft-delete; wallet records are persistent financial state.
/// </summary>
public sealed class Wallet
{
    /// <summary>
    /// Unique identifier for this wallet (UUID v7).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="User"/> who owns this wallet. Unique constraint.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Current wallet balance in paise (smallest currency unit). Must be non-negative.
    /// </summary>
    public int BalancePaise { get; set; }

    /// <summary>
    /// Timestamp when this wallet was created (UTC).
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when this wallet was last updated (UTC).
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// The user who owns this wallet.
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// All transactions associated with this wallet.
    /// </summary>
    public ICollection<WalletTransaction> Transactions { get; set; } = [];

    // ───────────────────────── Factory & Domain Methods ─────────────────────────

    /// <summary>
    /// Creates a new wallet for the given user with zero balance.
    /// </summary>
    public static Wallet Create(Guid userId)
    {
        var now = DateTimeOffset.UtcNow;
        return new Wallet
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            BalancePaise = 0,
            CreatedAt = now,
            UpdatedAt = now,
        };
    }

    /// <summary>
    /// Credits the wallet with the specified amount in paise.
    /// </summary>
    public void Credit(int amountPaise)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amountPaise);
        BalancePaise += amountPaise;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Debits the wallet by the specified amount in paise.
    /// Throws if balance is insufficient.
    /// </summary>
    public void Debit(int amountPaise)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amountPaise);
        if (BalancePaise < amountPaise)
        {
            throw new InvalidOperationException(
                $"Insufficient wallet balance. Available: {BalancePaise}, Requested: {amountPaise}");
        }

        BalancePaise -= amountPaise;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
