using FluentAssertions;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Subscriptions.Commands.SubscribeToPlan;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.UnitTests.Common;

namespace SwiggyClone.UnitTests.Features.Subscriptions;

public sealed class SubscribeToPlanCommandHandlerTests
{
    private readonly IAppDbContext _db;
    private readonly SubscribeToPlanCommandHandler _handler;

    private readonly List<SubscriptionPlan> _plans = [];
    private readonly List<UserSubscription> _subscriptions = [];

    private static readonly Guid PlanId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public SubscribeToPlanCommandHandlerTests()
    {
        _db = MockDbContextFactory.Create(
            subscriptionPlans: _plans,
            userSubscriptions: _subscriptions);

        _handler = new SubscribeToPlanCommandHandler(_db);
    }

    private static SubscriptionPlan CreateActivePlan() => new()
    {
        Id = PlanId,
        Name = "ZipMeal Pro",
        Description = "Premium plan with free delivery",
        PricePaise = 49900,
        DurationDays = 30,
        FreeDelivery = true,
        ExtraDiscountPercent = 10,
        NoSurgeFee = true,
        IsActive = true,
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow,
    };

    [Fact]
    public async Task Handle_PlanNotFound_ReturnsFailure()
    {
        // Arrange: no plans in DB
        var command = new SubscribeToPlanCommand(TestConstants.UserId, PlanId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("PLAN_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_InactivePlan_ReturnsFailure()
    {
        // Arrange: plan exists but is inactive
        var plan = CreateActivePlan();
        plan.IsActive = false;
        _plans.Add(plan);

        var command = new SubscribeToPlanCommand(TestConstants.UserId, PlanId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("PLAN_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_AlreadySubscribed_ReturnsFailure()
    {
        // Arrange: plan is active, user already has an active subscription
        var plan = CreateActivePlan();
        _plans.Add(plan);

        _subscriptions.Add(new UserSubscription
        {
            Id = Guid.NewGuid(),
            UserId = TestConstants.UserId,
            PlanId = PlanId,
            StartDate = DateTimeOffset.UtcNow.AddDays(-5),
            EndDate = DateTimeOffset.UtcNow.AddDays(25),
            Status = SubscriptionStatus.Active,
            PaidAmountPaise = 49900,
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-5),
        });

        var command = new SubscribeToPlanCommand(TestConstants.UserId, PlanId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("ALREADY_SUBSCRIBED");
    }

    [Fact]
    public async Task Handle_ExpiredSubscription_AllowsNewSubscription()
    {
        // Arrange: user has an expired subscription (EndDate in the past)
        var plan = CreateActivePlan();
        _plans.Add(plan);

        _subscriptions.Add(new UserSubscription
        {
            Id = Guid.NewGuid(),
            UserId = TestConstants.UserId,
            PlanId = PlanId,
            StartDate = DateTimeOffset.UtcNow.AddDays(-60),
            EndDate = DateTimeOffset.UtcNow.AddDays(-30),
            Status = SubscriptionStatus.Active, // Still Active status but expired by date
            PaidAmountPaise = 49900,
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-60),
        });

        var command = new SubscribeToPlanCommand(TestConstants.UserId, PlanId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ValidSubscription_ReturnsSuccess()
    {
        // Arrange
        var plan = CreateActivePlan();
        _plans.Add(plan);

        var command = new SubscribeToPlanCommand(TestConstants.UserId, PlanId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.PlanId.Should().Be(PlanId);
        result.Value.PlanName.Should().Be("ZipMeal Pro");
        result.Value.PaidAmountPaise.Should().Be(49900);
        result.Value.FreeDelivery.Should().BeTrue();
        result.Value.ExtraDiscountPercent.Should().Be(10);
        result.Value.NoSurgeFee.Should().BeTrue();
        result.Value.Status.Should().Be((int)SubscriptionStatus.Active);
    }

    [Fact]
    public async Task Handle_ValidSubscription_CreatesRecordAndSaves()
    {
        // Arrange
        var plan = CreateActivePlan();
        _plans.Add(plan);

        var command = new SubscribeToPlanCommand(TestConstants.UserId, PlanId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _subscriptions.Should().HaveCount(1);
        _subscriptions[0].UserId.Should().Be(TestConstants.UserId);
        _subscriptions[0].PlanId.Should().Be(PlanId);
        _subscriptions[0].Status.Should().Be(SubscriptionStatus.Active);
        _subscriptions[0].PaidAmountPaise.Should().Be(49900);
        await _db.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidSubscription_SetsCorrectEndDate()
    {
        // Arrange
        var plan = CreateActivePlan();
        plan.DurationDays = 90;
        _plans.Add(plan);

        var command = new SubscribeToPlanCommand(TestConstants.UserId, PlanId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        var expectedEnd = result.Value.StartDate.AddDays(90);
        result.Value.EndDate.Should().BeCloseTo(expectedEnd, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Handle_CancelledSubscription_AllowsNewSubscription()
    {
        // Arrange: user has a cancelled subscription
        var plan = CreateActivePlan();
        _plans.Add(plan);

        _subscriptions.Add(new UserSubscription
        {
            Id = Guid.NewGuid(),
            UserId = TestConstants.UserId,
            PlanId = PlanId,
            StartDate = DateTimeOffset.UtcNow.AddDays(-10),
            EndDate = DateTimeOffset.UtcNow.AddDays(20),
            Status = SubscriptionStatus.Cancelled,
            PaidAmountPaise = 49900,
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-10),
        });

        var command = new SubscribeToPlanCommand(TestConstants.UserId, PlanId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert: cancelled sub doesn't block new subscription
        result.IsSuccess.Should().BeTrue();
    }
}
