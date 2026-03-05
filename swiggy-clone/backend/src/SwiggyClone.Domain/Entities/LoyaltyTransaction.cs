using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Domain.Entities;

public sealed class LoyaltyTransaction
{
    public Guid Id { get; set; }
    public Guid LoyaltyAccountId { get; set; }
    public int Points { get; set; }
    public LoyaltyTransactionType Type { get; set; }
    public LoyaltyTransactionSource Source { get; set; }
    public Guid? ReferenceId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int BalanceAfter { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    public LoyaltyAccount Account { get; set; } = null!;

    // ───────────────────────── Factory ─────────────────────────

    public static LoyaltyTransaction Create(
        Guid loyaltyAccountId,
        int points,
        LoyaltyTransactionType type,
        LoyaltyTransactionSource source,
        Guid? referenceId,
        string description,
        int balanceAfter)
    {
        return new LoyaltyTransaction
        {
            Id = Guid.CreateVersion7(),
            LoyaltyAccountId = loyaltyAccountId,
            Points = points,
            Type = type,
            Source = source,
            ReferenceId = referenceId,
            Description = description,
            BalanceAfter = balanceAfter,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }
}
