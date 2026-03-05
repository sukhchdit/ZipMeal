using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Loyalty.Commands.EarnPoints;
using SwiggyClone.Application.Features.Loyalty.Commands.GetOrCreateLoyaltyAccount;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.UnitTests.Common;

namespace SwiggyClone.UnitTests.Features.Loyalty;

public sealed class EarnPointsCommandHandlerTests
{
    private static readonly Guid TestUserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid TestOrderId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    private static readonly Guid TestAccountId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

    private readonly ISender _sender = Substitute.For<ISender>();
    private readonly IEventBus _eventBus = Substitute.For<IEventBus>();
    private readonly ILogger<EarnPointsCommandHandler> _logger = Substitute.For<ILogger<EarnPointsCommandHandler>>();

    private static LoyaltyTier CreateBronzeTier() => new()
    {
        Id = Guid.NewGuid(),
        Level = LoyaltyTierLevel.Bronze,
        Name = "Bronze",
        MinLifetimePoints = 0,
        PointsPerHundredPaise = 1,
        Multiplier = 1.0,
    };

    private static LoyaltyTier CreateSilverTier() => new()
    {
        Id = Guid.NewGuid(),
        Level = LoyaltyTierLevel.Silver,
        Name = "Silver",
        MinLifetimePoints = 500,
        PointsPerHundredPaise = 2,
        Multiplier = 1.5,
    };

    private static Order CreateDeliveredOrder(Guid? orderId = null, int totalAmount = 500_00, int deliveryFee = 50_00) => new()
    {
        Id = orderId ?? TestOrderId,
        UserId = TestUserId,
        RestaurantId = Guid.NewGuid(),
        OrderNumber = "SWG-20260303-0001",
        Status = OrderStatus.Delivered,
        TotalAmount = totalAmount,
        DeliveryFee = deliveryFee,
        Subtotal = totalAmount - deliveryFee,
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow,
    };

    private static LoyaltyAccount CreateExistingAccount() => new()
    {
        Id = TestAccountId,
        UserId = TestUserId,
        PointsBalance = 100,
        LifetimePointsEarned = 100,
        CurrentTier = LoyaltyTierLevel.Bronze,
        LastActivityAt = DateTimeOffset.UtcNow,
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow,
    };

    [Fact]
    public async Task Handle_ValidOrder_EarnsPoints()
    {
        // Arrange
        var order = CreateDeliveredOrder();
        var account = CreateExistingAccount();
        var bronzeTier = CreateBronzeTier();

        var db = MockDbContextFactory.Create(
            orders: new List<Order> { order },
            loyaltyAccounts: new List<LoyaltyAccount> { account },
            loyaltyTiers: new List<LoyaltyTier> { bronzeTier },
            loyaltyTransactions: new List<LoyaltyTransaction>());

        _sender.Send(Arg.Any<GetOrCreateLoyaltyAccountCommand>(), Arg.Any<CancellationToken>())
            .Returns(account);

        var handler = new EarnPointsCommandHandler(db, _sender, _eventBus, _logger);
        var command = new EarnPointsCommand(TestUserId, TestOrderId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeGreaterThan(0);
        account.PointsBalance.Should().BeGreaterThan(100);
        account.LifetimePointsEarned.Should().BeGreaterThan(100);
        await db.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_AlreadyEarned_ReturnsFailure()
    {
        // Arrange
        var existingTransaction = new LoyaltyTransaction
        {
            Id = Guid.NewGuid(),
            LoyaltyAccountId = TestAccountId,
            Points = 10,
            Type = LoyaltyTransactionType.Earn,
            Source = LoyaltyTransactionSource.OrderDelivered,
            ReferenceId = TestOrderId,
            Description = "Earned 10 points for order delivery",
            BalanceAfter = 110,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        var order = CreateDeliveredOrder();

        var db = MockDbContextFactory.Create(
            orders: new List<Order> { order },
            loyaltyTransactions: new List<LoyaltyTransaction> { existingTransaction });

        var handler = new EarnPointsCommandHandler(db, _sender, _eventBus, _logger);
        var command = new EarnPointsCommand(TestUserId, TestOrderId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("LOYALTY_ALREADY_EARNED");
    }

    [Fact]
    public async Task Handle_OrderNotFound_ReturnsFailure()
    {
        // Arrange — no orders in the database
        var db = MockDbContextFactory.Create(
            orders: new List<Order>(),
            loyaltyTransactions: new List<LoyaltyTransaction>());

        var handler = new EarnPointsCommandHandler(db, _sender, _eventBus, _logger);
        var command = new EarnPointsCommand(TestUserId, TestOrderId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("ORDER_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_SubscriptionActive_EarnsMorePoints()
    {
        // Arrange
        var order = CreateDeliveredOrder(totalAmount: 1_000_00, deliveryFee: 0);
        var account = CreateExistingAccount();
        account.PointsBalance = 0;
        account.LifetimePointsEarned = 0;
        var bronzeTier = CreateBronzeTier();

        var activeSubscription = new UserSubscription
        {
            Id = Guid.NewGuid(),
            UserId = TestUserId,
            PlanId = Guid.NewGuid(),
            Status = SubscriptionStatus.Active,
            StartDate = DateTimeOffset.UtcNow.AddDays(-10),
            EndDate = DateTimeOffset.UtcNow.AddDays(20),
            PaidAmountPaise = 99_00,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        var db = MockDbContextFactory.Create(
            orders: new List<Order> { order },
            loyaltyAccounts: new List<LoyaltyAccount> { account },
            loyaltyTiers: new List<LoyaltyTier> { bronzeTier },
            loyaltyTransactions: new List<LoyaltyTransaction>(),
            userSubscriptions: new List<UserSubscription> { activeSubscription });

        _sender.Send(Arg.Any<GetOrCreateLoyaltyAccountCommand>(), Arg.Any<CancellationToken>())
            .Returns(account);

        var handlerWithSub = new EarnPointsCommandHandler(db, _sender, _eventBus, _logger);
        var command = new EarnPointsCommand(TestUserId, TestOrderId);

        // Act
        var result = await handlerWithSub.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        // With subscription, points should be multiplied by 1.5x
        // Base: 1000_00 / 10000 * 1 = 10, * 1.0 = 10, * 1.5 = 15
        result.Value.Should().Be(15);
    }

    [Fact]
    public async Task Handle_ValidOrder_PublishesKafkaEvent()
    {
        // Arrange
        var order = CreateDeliveredOrder();
        var account = CreateExistingAccount();
        var bronzeTier = CreateBronzeTier();

        var db = MockDbContextFactory.Create(
            orders: new List<Order> { order },
            loyaltyAccounts: new List<LoyaltyAccount> { account },
            loyaltyTiers: new List<LoyaltyTier> { bronzeTier },
            loyaltyTransactions: new List<LoyaltyTransaction>());

        _sender.Send(Arg.Any<GetOrCreateLoyaltyAccountCommand>(), Arg.Any<CancellationToken>())
            .Returns(account);

        var handler = new EarnPointsCommandHandler(db, _sender, _eventBus, _logger);
        var command = new EarnPointsCommand(TestUserId, TestOrderId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await _eventBus.Received(1).PublishAsync(
            "loyalty.points.earned",
            Arg.Any<string>(),
            Arg.Any<object>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidOrder_CreatesTranasctionRecord()
    {
        // Arrange
        var order = CreateDeliveredOrder();
        var account = CreateExistingAccount();
        var bronzeTier = CreateBronzeTier();
        var transactions = new List<LoyaltyTransaction>();

        var db = MockDbContextFactory.Create(
            orders: new List<Order> { order },
            loyaltyAccounts: new List<LoyaltyAccount> { account },
            loyaltyTiers: new List<LoyaltyTier> { bronzeTier },
            loyaltyTransactions: transactions);

        _sender.Send(Arg.Any<GetOrCreateLoyaltyAccountCommand>(), Arg.Any<CancellationToken>())
            .Returns(account);

        var handler = new EarnPointsCommandHandler(db, _sender, _eventBus, _logger);
        var command = new EarnPointsCommand(TestUserId, TestOrderId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        transactions.Should().HaveCount(1);
        transactions[0].Source.Should().Be(LoyaltyTransactionSource.OrderDelivered);
        transactions[0].Type.Should().Be(LoyaltyTransactionType.Earn);
        transactions[0].ReferenceId.Should().Be(TestOrderId);
    }
}
