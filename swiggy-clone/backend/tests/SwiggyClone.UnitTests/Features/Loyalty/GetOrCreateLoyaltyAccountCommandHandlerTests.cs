using FluentAssertions;
using SwiggyClone.Application.Features.Loyalty.Commands.GetOrCreateLoyaltyAccount;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.UnitTests.Common;

namespace SwiggyClone.UnitTests.Features.Loyalty;

public sealed class GetOrCreateLoyaltyAccountCommandHandlerTests
{
    private static readonly Guid TestUserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid TestAccountId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

    [Fact]
    public async Task Handle_AccountExists_ReturnsExisting()
    {
        // Arrange
        var existingAccount = new LoyaltyAccount
        {
            Id = TestAccountId,
            UserId = TestUserId,
            PointsBalance = 250,
            LifetimePointsEarned = 500,
            CurrentTier = LoyaltyTierLevel.Silver,
            LastActivityAt = DateTimeOffset.UtcNow,
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-30),
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        var db = MockDbContextFactory.Create(
            loyaltyAccounts: new List<LoyaltyAccount> { existingAccount });

        var handler = new GetOrCreateLoyaltyAccountCommandHandler(db);
        var command = new GetOrCreateLoyaltyAccountCommand(TestUserId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(TestAccountId);
        result.UserId.Should().Be(TestUserId);
        result.PointsBalance.Should().Be(250);
        result.CurrentTier.Should().Be(LoyaltyTierLevel.Silver);
        await db.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NoAccount_CreatesNew()
    {
        // Arrange
        var accountsList = new List<LoyaltyAccount>();
        var db = MockDbContextFactory.Create(
            loyaltyAccounts: accountsList);

        var handler = new GetOrCreateLoyaltyAccountCommandHandler(db);
        var command = new GetOrCreateLoyaltyAccountCommand(TestUserId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(TestUserId);
        result.PointsBalance.Should().Be(0);
        result.LifetimePointsEarned.Should().Be(0);
        result.CurrentTier.Should().Be(LoyaltyTierLevel.Bronze);
        accountsList.Should().HaveCount(1);
        await db.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NoAccount_NewAccountHasValidTimestamps()
    {
        // Arrange
        var before = DateTimeOffset.UtcNow;
        var accountsList = new List<LoyaltyAccount>();
        var db = MockDbContextFactory.Create(
            loyaltyAccounts: accountsList);

        var handler = new GetOrCreateLoyaltyAccountCommandHandler(db);
        var command = new GetOrCreateLoyaltyAccountCommand(TestUserId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        var after = DateTimeOffset.UtcNow;

        // Assert
        result.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        result.UpdatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        result.LastActivityAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_MultipleUsers_ReturnsCorrectAccount()
    {
        // Arrange
        var otherUserId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
        var otherAccount = new LoyaltyAccount
        {
            Id = Guid.NewGuid(),
            UserId = otherUserId,
            PointsBalance = 999,
            LifetimePointsEarned = 999,
            CurrentTier = LoyaltyTierLevel.Platinum,
            LastActivityAt = DateTimeOffset.UtcNow,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        var targetAccount = new LoyaltyAccount
        {
            Id = TestAccountId,
            UserId = TestUserId,
            PointsBalance = 50,
            LifetimePointsEarned = 50,
            CurrentTier = LoyaltyTierLevel.Bronze,
            LastActivityAt = DateTimeOffset.UtcNow,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        var db = MockDbContextFactory.Create(
            loyaltyAccounts: new List<LoyaltyAccount> { otherAccount, targetAccount });

        var handler = new GetOrCreateLoyaltyAccountCommandHandler(db);
        var command = new GetOrCreateLoyaltyAccountCommand(TestUserId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.UserId.Should().Be(TestUserId);
        result.PointsBalance.Should().Be(50);
    }
}
