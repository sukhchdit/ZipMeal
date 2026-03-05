using FluentAssertions;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.UnitTests.Domain;

public sealed class LoyaltyRewardTests
{
    private static LoyaltyReward CreateReward(
        bool isActive = true,
        DateTimeOffset? expiresAt = null,
        int? stock = 10) => new()
    {
        Id = Guid.NewGuid(),
        Name = "Test Reward",
        Description = "A test loyalty reward",
        PointsCost = 100,
        RewardType = LoyaltyRewardType.WalletCredit,
        RewardValue = 50_00,
        IsActive = isActive,
        Stock = stock,
        ExpiresAt = expiresAt,
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow,
    };

    [Fact]
    public void IsRedeemable_ActiveWithStock_ReturnsTrue()
    {
        // Arrange
        var reward = CreateReward(
            isActive: true,
            expiresAt: DateTimeOffset.UtcNow.AddDays(30),
            stock: 5);

        // Act
        var result = reward.IsRedeemable();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsRedeemable_ActiveNoExpiryNoStock_ReturnsTrue()
    {
        // Arrange — null stock and null expiry means unlimited
        var reward = CreateReward(
            isActive: true,
            expiresAt: null,
            stock: null);

        // Act
        var result = reward.IsRedeemable();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsRedeemable_Inactive_ReturnsFalse()
    {
        // Arrange
        var reward = CreateReward(
            isActive: false,
            expiresAt: DateTimeOffset.UtcNow.AddDays(30),
            stock: 10);

        // Act
        var result = reward.IsRedeemable();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsRedeemable_Expired_ReturnsFalse()
    {
        // Arrange
        var reward = CreateReward(
            isActive: true,
            expiresAt: DateTimeOffset.UtcNow.AddDays(-1),
            stock: 10);

        // Act
        var result = reward.IsRedeemable();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsRedeemable_OutOfStock_ReturnsFalse()
    {
        // Arrange
        var reward = CreateReward(
            isActive: true,
            expiresAt: DateTimeOffset.UtcNow.AddDays(30),
            stock: 0);

        // Act
        var result = reward.IsRedeemable();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsRedeemable_NegativeStock_ReturnsFalse()
    {
        // Arrange
        var reward = CreateReward(
            isActive: true,
            expiresAt: DateTimeOffset.UtcNow.AddDays(30),
            stock: -1);

        // Act
        var result = reward.IsRedeemable();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void DecrementStock_DecrementsValue()
    {
        // Arrange
        var reward = CreateReward(stock: 5);

        // Act
        reward.DecrementStock();

        // Assert
        reward.Stock.Should().Be(4);
    }

    [Fact]
    public void DecrementStock_MultipleDecrements_DecrementsCorrectly()
    {
        // Arrange
        var reward = CreateReward(stock: 3);

        // Act
        reward.DecrementStock();
        reward.DecrementStock();

        // Assert
        reward.Stock.Should().Be(1);
    }

    [Fact]
    public void DecrementStock_NullStock_DoesNothing()
    {
        // Arrange — null stock means unlimited
        var reward = CreateReward(stock: null);

        // Act
        reward.DecrementStock();

        // Assert
        reward.Stock.Should().BeNull();
    }

    [Fact]
    public void DecrementStock_UpdatesTimestamp()
    {
        // Arrange
        var reward = CreateReward(stock: 5);
        var before = DateTimeOffset.UtcNow;

        // Act
        reward.DecrementStock();

        // Assert
        reward.UpdatedAt.Should().BeOnOrAfter(before);
    }

    [Fact]
    public void DecrementStock_FromOne_ReachesZero()
    {
        // Arrange
        var reward = CreateReward(stock: 1);

        // Act
        reward.DecrementStock();

        // Assert
        reward.Stock.Should().Be(0);
        reward.IsRedeemable().Should().BeFalse();
    }
}
