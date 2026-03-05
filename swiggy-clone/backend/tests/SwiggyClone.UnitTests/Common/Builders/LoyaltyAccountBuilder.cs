using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.UnitTests.Common.Builders;

public sealed class LoyaltyAccountBuilder
{
    private Guid _id = TestConstants.LoyaltyAccountId;
    private Guid _userId = TestConstants.UserId;
    private int _pointsBalance;
    private int _lifetimePointsEarned;
    private LoyaltyTierLevel _currentTier = LoyaltyTierLevel.Bronze;

    public LoyaltyAccountBuilder WithId(Guid id) { _id = id; return this; }
    public LoyaltyAccountBuilder WithUserId(Guid userId) { _userId = userId; return this; }
    public LoyaltyAccountBuilder WithPointsBalance(int balance) { _pointsBalance = balance; return this; }
    public LoyaltyAccountBuilder WithLifetimePointsEarned(int points) { _lifetimePointsEarned = points; return this; }
    public LoyaltyAccountBuilder WithCurrentTier(LoyaltyTierLevel tier) { _currentTier = tier; return this; }

    public LoyaltyAccount Build() => new()
    {
        Id = _id,
        UserId = _userId,
        PointsBalance = _pointsBalance,
        LifetimePointsEarned = _lifetimePointsEarned,
        CurrentTier = _currentTier,
        LastActivityAt = DateTimeOffset.UtcNow,
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow,
    };
}
