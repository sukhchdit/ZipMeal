using FluentAssertions;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.UnitTests.Domain;

public sealed class LoyaltyAccountTests
{
    private static readonly Guid TestUserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    private static LoyaltyTier CreateTier(LoyaltyTierLevel level, int minPoints) => new()
    {
        Id = Guid.NewGuid(),
        Level = level,
        Name = level.ToString(),
        MinLifetimePoints = minPoints,
        PointsPerHundredPaise = (int)level + 1,
        Multiplier = 1.0 + (int)level * 0.5,
    };

    [Fact]
    public void Create_NewAccount_HasCorrectDefaults()
    {
        // Act
        var account = LoyaltyAccount.Create(TestUserId);

        // Assert
        account.Id.Should().NotBeEmpty();
        account.UserId.Should().Be(TestUserId);
        account.PointsBalance.Should().Be(0);
        account.LifetimePointsEarned.Should().Be(0);
        account.CurrentTier.Should().Be(LoyaltyTierLevel.Bronze);
        account.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
        account.UpdatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
        account.LastActivityAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void EarnPoints_ValidAmount_IncreasesBalance()
    {
        // Arrange
        var account = LoyaltyAccount.Create(TestUserId);
        var initialBalance = account.PointsBalance;

        // Act
        account.EarnPoints(50);

        // Assert
        account.PointsBalance.Should().Be(initialBalance + 50);
        account.LifetimePointsEarned.Should().Be(50);
    }

    [Fact]
    public void EarnPoints_MultipleCalls_AccumulatesCorrectly()
    {
        // Arrange
        var account = LoyaltyAccount.Create(TestUserId);

        // Act
        account.EarnPoints(30);
        account.EarnPoints(20);
        account.EarnPoints(50);

        // Assert
        account.PointsBalance.Should().Be(100);
        account.LifetimePointsEarned.Should().Be(100);
    }

    [Fact]
    public void EarnPoints_Zero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var account = LoyaltyAccount.Create(TestUserId);

        // Act
        var act = () => account.EarnPoints(0);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void EarnPoints_Negative_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var account = LoyaltyAccount.Create(TestUserId);

        // Act
        var act = () => account.EarnPoints(-10);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void EarnPoints_ValidAmount_UpdatesLastActivityAt()
    {
        // Arrange
        var account = LoyaltyAccount.Create(TestUserId);
        var before = DateTimeOffset.UtcNow;

        // Act
        account.EarnPoints(10);

        // Assert
        account.LastActivityAt.Should().BeOnOrAfter(before);
        account.UpdatedAt.Should().BeOnOrAfter(before);
    }

    [Fact]
    public void RedeemPoints_SufficientBalance_DeductsPoints()
    {
        // Arrange
        var account = LoyaltyAccount.Create(TestUserId);
        account.EarnPoints(100);

        // Act
        account.RedeemPoints(40);

        // Assert
        account.PointsBalance.Should().Be(60);
        // LifetimePointsEarned should not be affected by redemptions
        account.LifetimePointsEarned.Should().Be(100);
    }

    [Fact]
    public void RedeemPoints_ExactBalance_LeavesZero()
    {
        // Arrange
        var account = LoyaltyAccount.Create(TestUserId);
        account.EarnPoints(50);

        // Act
        account.RedeemPoints(50);

        // Assert
        account.PointsBalance.Should().Be(0);
    }

    [Fact]
    public void RedeemPoints_InsufficientBalance_ThrowsInvalidOperationException()
    {
        // Arrange
        var account = LoyaltyAccount.Create(TestUserId);
        account.EarnPoints(30);

        // Act
        var act = () => account.RedeemPoints(50);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Insufficient*");
    }

    [Fact]
    public void RedeemPoints_Zero_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var account = LoyaltyAccount.Create(TestUserId);
        account.EarnPoints(100);

        // Act
        var act = () => account.RedeemPoints(0);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void RedeemPoints_Negative_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var account = LoyaltyAccount.Create(TestUserId);
        account.EarnPoints(100);

        // Act
        var act = () => account.RedeemPoints(-5);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ExpirePoints_SetsBalanceToZero()
    {
        // Arrange
        var account = LoyaltyAccount.Create(TestUserId);
        account.EarnPoints(500);

        // Act
        account.ExpirePoints();

        // Assert
        account.PointsBalance.Should().Be(0);
        // LifetimePointsEarned should remain unchanged
        account.LifetimePointsEarned.Should().Be(500);
    }

    [Fact]
    public void ExpirePoints_AlreadyZero_RemainsZero()
    {
        // Arrange
        var account = LoyaltyAccount.Create(TestUserId);

        // Act
        account.ExpirePoints();

        // Assert
        account.PointsBalance.Should().Be(0);
    }

    [Fact]
    public void ExpirePoints_UpdatesTimestamp()
    {
        // Arrange
        var account = LoyaltyAccount.Create(TestUserId);
        account.EarnPoints(100);
        var before = DateTimeOffset.UtcNow;

        // Act
        account.ExpirePoints();

        // Assert
        account.UpdatedAt.Should().BeOnOrAfter(before);
    }

    [Fact]
    public void RecalculateTier_HigherTier_Upgrades()
    {
        // Arrange
        var account = LoyaltyAccount.Create(TestUserId);
        account.EarnPoints(600); // Enough for Silver (500+ lifetime points)

        var tiers = new List<LoyaltyTier>
        {
            CreateTier(LoyaltyTierLevel.Bronze, 0),
            CreateTier(LoyaltyTierLevel.Silver, 500),
            CreateTier(LoyaltyTierLevel.Gold, 2000),
            CreateTier(LoyaltyTierLevel.Platinum, 5000),
        };

        // Act
        account.RecalculateTier(tiers);

        // Assert
        account.CurrentTier.Should().Be(LoyaltyTierLevel.Silver);
    }

    [Fact]
    public void RecalculateTier_LowerTier_DoesNotDowngrade()
    {
        // Arrange
        var account = LoyaltyAccount.Create(TestUserId);
        account.EarnPoints(600);
        account.CurrentTier = LoyaltyTierLevel.Gold; // Set to higher tier manually

        var tiers = new List<LoyaltyTier>
        {
            CreateTier(LoyaltyTierLevel.Bronze, 0),
            CreateTier(LoyaltyTierLevel.Silver, 500),
            CreateTier(LoyaltyTierLevel.Gold, 2000),
            CreateTier(LoyaltyTierLevel.Platinum, 5000),
        };

        // Act
        account.RecalculateTier(tiers);

        // Assert — should remain Gold, not downgrade to Silver
        account.CurrentTier.Should().Be(LoyaltyTierLevel.Gold);
    }

    [Fact]
    public void RecalculateTier_ExactMinPoints_UpgradesToThat()
    {
        // Arrange
        var account = LoyaltyAccount.Create(TestUserId);
        account.EarnPoints(2000);

        var tiers = new List<LoyaltyTier>
        {
            CreateTier(LoyaltyTierLevel.Bronze, 0),
            CreateTier(LoyaltyTierLevel.Silver, 500),
            CreateTier(LoyaltyTierLevel.Gold, 2000),
            CreateTier(LoyaltyTierLevel.Platinum, 5000),
        };

        // Act
        account.RecalculateTier(tiers);

        // Assert
        account.CurrentTier.Should().Be(LoyaltyTierLevel.Gold);
    }

    [Fact]
    public void RecalculateTier_NotEnoughForNextTier_StaysAtCurrent()
    {
        // Arrange
        var account = LoyaltyAccount.Create(TestUserId);
        account.EarnPoints(100); // Not enough for Silver (500)

        var tiers = new List<LoyaltyTier>
        {
            CreateTier(LoyaltyTierLevel.Bronze, 0),
            CreateTier(LoyaltyTierLevel.Silver, 500),
            CreateTier(LoyaltyTierLevel.Gold, 2000),
        };

        // Act
        account.RecalculateTier(tiers);

        // Assert
        account.CurrentTier.Should().Be(LoyaltyTierLevel.Bronze);
    }

    [Fact]
    public void RecalculateTier_EnoughForPlatinum_UpgradesToPlatinum()
    {
        // Arrange
        var account = LoyaltyAccount.Create(TestUserId);
        account.EarnPoints(10000);

        var tiers = new List<LoyaltyTier>
        {
            CreateTier(LoyaltyTierLevel.Bronze, 0),
            CreateTier(LoyaltyTierLevel.Silver, 500),
            CreateTier(LoyaltyTierLevel.Gold, 2000),
            CreateTier(LoyaltyTierLevel.Platinum, 5000),
        };

        // Act
        account.RecalculateTier(tiers);

        // Assert
        account.CurrentTier.Should().Be(LoyaltyTierLevel.Platinum);
    }
}
