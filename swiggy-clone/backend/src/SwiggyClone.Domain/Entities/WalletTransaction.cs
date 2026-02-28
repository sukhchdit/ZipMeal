using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents an immutable audit trail entry for a wallet credit or debit.
/// Each transaction records the amount, type, source, and the resulting balance.
/// This entity does not support soft-delete; financial records are immutable.
/// </summary>
public sealed class WalletTransaction
{
    /// <summary>
    /// Unique identifier for this transaction (UUID v7).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the <see cref="Wallet"/> this transaction belongs to.
    /// </summary>
    public Guid WalletId { get; set; }

    /// <summary>
    /// Transaction amount in paise. Always positive; direction is indicated by <see cref="Type"/>.
    /// </summary>
    public int AmountPaise { get; set; }

    /// <summary>
    /// Whether this is a credit or debit transaction.
    /// </summary>
    public WalletTransactionType Type { get; set; }

    /// <summary>
    /// The business source that triggered this transaction (AddMoney, OrderPayment, Refund, etc.).
    /// </summary>
    public WalletTransactionSource Source { get; set; }

    /// <summary>
    /// Optional reference to a related entity (orderId, couponId, etc.).
    /// </summary>
    public Guid? ReferenceId { get; set; }

    /// <summary>
    /// Human-readable description of this transaction (max 500 characters).
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Wallet balance in paise after this transaction was applied.
    /// Enables showing a running balance without re-querying.
    /// </summary>
    public int BalanceAfterPaise { get; set; }

    /// <summary>
    /// Timestamp when this transaction was created (UTC).
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    /// <summary>
    /// The wallet this transaction belongs to.
    /// </summary>
    public Wallet Wallet { get; set; } = null!;

    // ───────────────────────── Factory ─────────────────────────

    /// <summary>
    /// Creates a new wallet transaction record.
    /// </summary>
    public static WalletTransaction Create(
        Guid walletId,
        int amountPaise,
        WalletTransactionType type,
        WalletTransactionSource source,
        Guid? referenceId,
        string description,
        int balanceAfterPaise)
    {
        return new WalletTransaction
        {
            Id = Guid.CreateVersion7(),
            WalletId = walletId,
            AmountPaise = amountPaise,
            Type = type,
            Source = source,
            ReferenceId = referenceId,
            Description = description,
            BalanceAfterPaise = balanceAfterPaise,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }
}
