using FluentAssertions;
using MediatR;
using NSubstitute;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.GroupOrders.Commands;
using SwiggyClone.Application.Features.GroupOrders.Notifications;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.UnitTests.Common;
using SwiggyClone.UnitTests.Common.Builders;

namespace SwiggyClone.UnitTests.Features.GroupOrders;

public sealed class CreateGroupOrderCommandHandlerTests
{
    private readonly IPublisher _publisher;
    private readonly List<GroupOrder> _groupOrders = [];
    private readonly List<GroupOrderParticipant> _groupOrderParticipants = [];
    private readonly List<Restaurant> _restaurants;
    private readonly List<User> _users;
    private readonly IAppDbContext _db;
    private readonly CreateGroupOrderCommandHandler _handler;

    public CreateGroupOrderCommandHandlerTests()
    {
        _publisher = Substitute.For<IPublisher>();

        var restaurant = new RestaurantBuilder().Build();
        _restaurants = [restaurant];

        var user = new UserBuilder().Build();
        _users = [user];

        _db = MockDbContextFactory.Create(
            users: _users,
            restaurants: _restaurants,
            groupOrders: _groupOrders,
            groupOrderParticipants: _groupOrderParticipants);

        _handler = new CreateGroupOrderCommandHandler(_db, _publisher);
    }

    private static CreateGroupOrderCommand ValidCommand() =>
        new(TestConstants.UserId, TestConstants.RestaurantId, PaymentSplitType.SplitEqual, TestConstants.AddressId);

    [Fact]
    public async Task Handle_ValidRequest_CreatesGroupOrder()
    {
        var command = ValidCommand();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.RestaurantId.Should().Be(TestConstants.RestaurantId);
        result.Value.InitiatorUserId.Should().Be(TestConstants.UserId);
        result.Value.Status.Should().Be(GroupOrderStatus.Active);
        result.Value.PaymentSplitType.Should().Be(PaymentSplitType.SplitEqual);
        result.Value.Participants.Should().HaveCount(1);
        result.Value.Participants[0].IsInitiator.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_RestaurantNotFound_ReturnsFailure()
    {
        var command = ValidCommand() with { RestaurantId = Guid.NewGuid() };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("RESTAURANT_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_RestaurantNotApproved_ReturnsFailure()
    {
        _restaurants.Clear();
        _restaurants.Add(new RestaurantBuilder().WithStatus(RestaurantStatus.Pending).Build());

        var command = ValidCommand();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("RESTAURANT_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_RestaurantNotAccepting_ReturnsFailure()
    {
        _restaurants.Clear();
        var closedRestaurant = new RestaurantBuilder().Build();
        closedRestaurant.IsAcceptingOrders = false;
        _restaurants.Add(closedRestaurant);

        var command = ValidCommand();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("RESTAURANT_CLOSED");
    }

    [Fact]
    public async Task Handle_UserHasActiveGroupOrder_ReturnsFailure()
    {
        var existingGroupOrder = new GroupOrderBuilder()
            .WithId(Guid.NewGuid())
            .WithStatus(GroupOrderStatus.Active)
            .Build();
        _groupOrders.Add(existingGroupOrder);

        var participant = new GroupOrderParticipant
        {
            Id = Guid.NewGuid(),
            GroupOrderId = existingGroupOrder.Id,
            UserId = TestConstants.UserId,
            IsInitiator = true,
            Status = GroupOrderParticipantStatus.Joined,
            JoinedAt = DateTimeOffset.UtcNow,
            GroupOrder = existingGroupOrder,
        };
        _groupOrderParticipants.Add(participant);

        var command = ValidCommand();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("GROUP_ORDER_ALREADY_EXISTS");
    }

    [Fact]
    public async Task Handle_Success_GeneratesInviteCode()
    {
        var command = ValidCommand();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.InviteCode.Should().NotBeNullOrEmpty();
        result.Value.InviteCode.Should().HaveLength(6);
    }

    [Fact]
    public async Task Handle_Success_SavesChanges()
    {
        var command = ValidCommand();

        await _handler.Handle(command, CancellationToken.None);

        await _db.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        _groupOrders.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_Success_PublishesNotification()
    {
        var command = ValidCommand();

        await _handler.Handle(command, CancellationToken.None);

        await _publisher.Received(1).Publish(
            Arg.Is<GroupOrderCreatedNotification>(n =>
                n.RestaurantId == TestConstants.RestaurantId
                && n.InitiatorUserId == TestConstants.UserId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Success_SetsExpiryToSixtyMinutes()
    {
        var command = ValidCommand();
        var before = DateTimeOffset.UtcNow.AddMinutes(59);

        var result = await _handler.Handle(command, CancellationToken.None);

        var after = DateTimeOffset.UtcNow.AddMinutes(61);
        result.Value.ExpiresAt.Should().BeAfter(before);
        result.Value.ExpiresAt.Should().BeBefore(after);
    }

    [Fact]
    public async Task Handle_UserLeftPreviousGroupOrder_CreatesNewGroupOrder()
    {
        var existingGroupOrder = new GroupOrderBuilder()
            .WithId(Guid.NewGuid())
            .WithStatus(GroupOrderStatus.Active)
            .Build();
        _groupOrders.Add(existingGroupOrder);

        var leftParticipant = new GroupOrderParticipant
        {
            Id = Guid.NewGuid(),
            GroupOrderId = existingGroupOrder.Id,
            UserId = TestConstants.UserId,
            IsInitiator = false,
            Status = GroupOrderParticipantStatus.Left,
            JoinedAt = DateTimeOffset.UtcNow.AddMinutes(-30),
            LeftAt = DateTimeOffset.UtcNow.AddMinutes(-10),
            GroupOrder = existingGroupOrder,
        };
        _groupOrderParticipants.Add(leftParticipant);

        var command = ValidCommand();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
}
