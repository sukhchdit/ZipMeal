using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Loyalty.Commands.GetOrCreateLoyaltyAccount;
using SwiggyClone.Application.Features.Loyalty.Commands.RedeemReward;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.UnitTests.Common;

namespace SwiggyClone.UnitTests.Features.Loyalty;

public sealed class RedeemRewardCommandHandlerTests
{
    private static readonly Guid TestUserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid TestAccountId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
    private static readonly Guid TestRewardId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");

    private readonly ISender _sender = Substitute.For<ISender>();
    private readonly IEventBus _eventBus = Substitute.For<IEventBus>();
    private readonly ILogger<RedeemRewardCommandHandler> _logger = Substitute.For<ILogger<RedeemRewardCommandHandler>>();

    private static LoyaltyAccount CreateAccountWithPoints(int points) => new()
    {
        Id = TestAccountId,
        UserId = TestUserId,
        PointsBalance = points,
        LifetimePointsEarned = points,
        CurrentTier = LoyaltyTierLevel.Bronze,
        LastActivityAt = DateTimeOffset.UtcNow,
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow,
    };

    private static LoyaltyReward CreateActiveReward(int pointsCost = 50, int? stock = 10) => new()
    {
        Id = TestRewardId,
        Name = "Free Delivery",
        Description = "One free delivery on your next order",
        PointsCost = pointsCost,
        RewardType = LoyaltyRewardType.FreeDelivery,
        RewardValue = 1,
        IsActive = true,
        Stock = stock,
        ExpiresAt = DateTimeOffset.UtcNow.AddDays(30),
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow,
    };

    [Fact]
    public async Task Handle_ValidReward_RedeemsSuccessfully()
    {
        // Arrange
        var account = CreateAccountWithPoints(200);
        var reward = CreateActiveReward(pointsCost: 50, stock: 5);

        var db = MockDbContextFactory.Create(
            loyaltyAccounts: new List<LoyaltyAccount> { account },
            loyaltyRewards: new List<LoyaltyReward> { reward },
            loyaltyTransactions: new List<LoyaltyTransaction>());

        _sender.Send(Arg.Any<GetOrCreateLoyaltyAccountCommand>(), Arg.Any<CancellationToken>())
            .Returns(account);

        var handler = new RedeemRewardCommandHandler(db, _sender, _eventBus, _logger);
        var command = new RedeemRewardCommand(TestUserId, TestRewardId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Points.Should().Be(50);
        account.PointsBalance.Should().Be(150);
        reward.Stock.Should().Be(4);
        await db.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_InsufficientPoints_ReturnsFailure()
    {
        // Arrange
        var account = CreateAccountWithPoints(30);
        var reward = CreateActiveReward(pointsCost: 50);

        var db = MockDbContextFactory.Create(
            loyaltyAccounts: new List<LoyaltyAccount> { account },
            loyaltyRewards: new List<LoyaltyReward> { reward },
            loyaltyTransactions: new List<LoyaltyTransaction>());

        _sender.Send(Arg.Any<GetOrCreateLoyaltyAccountCommand>(), Arg.Any<CancellationToken>())
            .Returns(account);

        var handler = new RedeemRewardCommandHandler(db, _sender, _eventBus, _logger);
        var command = new RedeemRewardCommand(TestUserId, TestRewardId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("LOYALTY_INSUFFICIENT_POINTS");
    }

    [Fact]
    public async Task Handle_RewardNotRedeemable_ReturnsFailure()
    {
        // Arrange — reward is inactive
        var account = CreateAccountWithPoints(200);
        var reward = CreateActiveReward(pointsCost: 50);
        reward.IsActive = false;

        var db = MockDbContextFactory.Create(
            loyaltyAccounts: new List<LoyaltyAccount> { account },
            loyaltyRewards: new List<LoyaltyReward> { reward },
            loyaltyTransactions: new List<LoyaltyTransaction>());

        _sender.Send(Arg.Any<GetOrCreateLoyaltyAccountCommand>(), Arg.Any<CancellationToken>())
            .Returns(account);

        var handler = new RedeemRewardCommandHandler(db, _sender, _eventBus, _logger);
        var command = new RedeemRewardCommand(TestUserId, TestRewardId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("LOYALTY_REWARD_UNAVAILABLE");
    }

    [Fact]
    public async Task Handle_RewardNotFound_ReturnsFailure()
    {
        // Arrange — empty rewards list
        var account = CreateAccountWithPoints(200);

        var db = MockDbContextFactory.Create(
            loyaltyAccounts: new List<LoyaltyAccount> { account },
            loyaltyRewards: new List<LoyaltyReward>(),
            loyaltyTransactions: new List<LoyaltyTransaction>());

        _sender.Send(Arg.Any<GetOrCreateLoyaltyAccountCommand>(), Arg.Any<CancellationToken>())
            .Returns(account);

        var handler = new RedeemRewardCommandHandler(db, _sender, _eventBus, _logger);
        var command = new RedeemRewardCommand(TestUserId, TestRewardId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("LOYALTY_REWARD_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_ExpiredReward_ReturnsFailure()
    {
        // Arrange
        var account = CreateAccountWithPoints(200);
        var reward = CreateActiveReward(pointsCost: 50);
        reward.ExpiresAt = DateTimeOffset.UtcNow.AddDays(-1);

        var db = MockDbContextFactory.Create(
            loyaltyAccounts: new List<LoyaltyAccount> { account },
            loyaltyRewards: new List<LoyaltyReward> { reward },
            loyaltyTransactions: new List<LoyaltyTransaction>());

        _sender.Send(Arg.Any<GetOrCreateLoyaltyAccountCommand>(), Arg.Any<CancellationToken>())
            .Returns(account);

        var handler = new RedeemRewardCommandHandler(db, _sender, _eventBus, _logger);
        var command = new RedeemRewardCommand(TestUserId, TestRewardId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("LOYALTY_REWARD_UNAVAILABLE");
    }

    [Fact]
    public async Task Handle_OutOfStockReward_ReturnsFailure()
    {
        // Arrange
        var account = CreateAccountWithPoints(200);
        var reward = CreateActiveReward(pointsCost: 50, stock: 0);

        var db = MockDbContextFactory.Create(
            loyaltyAccounts: new List<LoyaltyAccount> { account },
            loyaltyRewards: new List<LoyaltyReward> { reward },
            loyaltyTransactions: new List<LoyaltyTransaction>());

        _sender.Send(Arg.Any<GetOrCreateLoyaltyAccountCommand>(), Arg.Any<CancellationToken>())
            .Returns(account);

        var handler = new RedeemRewardCommandHandler(db, _sender, _eventBus, _logger);
        var command = new RedeemRewardCommand(TestUserId, TestRewardId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        // Could be LOYALTY_REWARD_UNAVAILABLE (from IsRedeemable) or LOYALTY_REWARD_OUT_OF_STOCK
        result.ErrorCode.Should().BeOneOf("LOYALTY_REWARD_UNAVAILABLE", "LOYALTY_REWARD_OUT_OF_STOCK");
    }

    [Fact]
    public async Task Handle_WalletCreditReward_SendsCreditWalletCommand()
    {
        // Arrange
        var account = CreateAccountWithPoints(200);
        var reward = new LoyaltyReward
        {
            Id = TestRewardId,
            Name = "Wallet Credit 50",
            Description = "Get 50 rupees wallet credit",
            PointsCost = 100,
            RewardType = LoyaltyRewardType.WalletCredit,
            RewardValue = 50_00,
            IsActive = true,
            Stock = 10,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(30),
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        var db = MockDbContextFactory.Create(
            loyaltyAccounts: new List<LoyaltyAccount> { account },
            loyaltyRewards: new List<LoyaltyReward> { reward },
            loyaltyTransactions: new List<LoyaltyTransaction>());

        _sender.Send(Arg.Any<GetOrCreateLoyaltyAccountCommand>(), Arg.Any<CancellationToken>())
            .Returns(account);

        var handler = new RedeemRewardCommandHandler(db, _sender, _eventBus, _logger);
        var command = new RedeemRewardCommand(TestUserId, TestRewardId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert — verify CreditWalletCommand was sent via ISender
        await _sender.Received(1).Send(
            Arg.Is<SwiggyClone.Application.Features.Wallet.Commands.CreditWallet.CreditWalletCommand>(
                c => c.UserId == TestUserId && c.AmountPaise == 50_00),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidReward_PublishesKafkaEvent()
    {
        // Arrange
        var account = CreateAccountWithPoints(200);
        var reward = CreateActiveReward(pointsCost: 50);

        var db = MockDbContextFactory.Create(
            loyaltyAccounts: new List<LoyaltyAccount> { account },
            loyaltyRewards: new List<LoyaltyReward> { reward },
            loyaltyTransactions: new List<LoyaltyTransaction>());

        _sender.Send(Arg.Any<GetOrCreateLoyaltyAccountCommand>(), Arg.Any<CancellationToken>())
            .Returns(account);

        var handler = new RedeemRewardCommandHandler(db, _sender, _eventBus, _logger);
        var command = new RedeemRewardCommand(TestUserId, TestRewardId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await _eventBus.Received(1).PublishAsync(
            "loyalty.reward.redeemed",
            Arg.Any<string>(),
            Arg.Any<object>(),
            Arg.Any<CancellationToken>());
    }
}
