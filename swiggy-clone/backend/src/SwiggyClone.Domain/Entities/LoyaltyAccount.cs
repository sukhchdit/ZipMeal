using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Domain.Entities;

public sealed class LoyaltyAccount
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int PointsBalance { get; set; }
    public int LifetimePointsEarned { get; set; }
    public LoyaltyTierLevel CurrentTier { get; set; }
    public DateTimeOffset LastActivityAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    public User User { get; set; } = null!;
    public ICollection<LoyaltyTransaction> Transactions { get; set; } = [];

    // ───────────────────────── Factory & Domain Methods ─────────────────────────

    public static LoyaltyAccount Create(Guid userId)
    {
        var now = DateTimeOffset.UtcNow;
        return new LoyaltyAccount
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            PointsBalance = 0,
            LifetimePointsEarned = 0,
            CurrentTier = LoyaltyTierLevel.Bronze,
            LastActivityAt = now,
            CreatedAt = now,
            UpdatedAt = now,
        };
    }

    public void EarnPoints(int points)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(points);
        PointsBalance += points;
        LifetimePointsEarned += points;
        LastActivityAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void RedeemPoints(int points)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(points);
        if (PointsBalance < points)
        {
            throw new InvalidOperationException(
                $"Insufficient loyalty points. Available: {PointsBalance}, Requested: {points}");
        }

        PointsBalance -= points;
        LastActivityAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void ExpirePoints()
    {
        PointsBalance = 0;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void RecalculateTier(IReadOnlyList<LoyaltyTier> tiers)
    {
        var newTier = LoyaltyTierLevel.Bronze;
        foreach (var tier in tiers)
        {
            if (LifetimePointsEarned >= tier.MinLifetimePoints && tier.Level >= newTier)
            {
                newTier = tier.Level;
            }
        }

        // Only upgrade, never downgrade in v1
        if (newTier > CurrentTier)
        {
            CurrentTier = newTier;
            UpdatedAt = DateTimeOffset.UtcNow;
        }
    }
}
