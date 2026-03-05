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

public sealed class JoinGroupOrderCommandHandlerTests
{
    private readonly IPublisher _publisher;
    private readonly List<GroupOrder> _groupOrders;
    private readonly List<GroupOrderParticipant> _groupOrderParticipants;
    private readonly List<User> _users;
    private readonly IAppDbContext _db;
    private readonly JoinGroupOrderCommandHandler _handler;

    public JoinGroupOrderCommandHandlerTests()
    {
        _publisher = Substitute.For<IPublisher>();

        var initiatorUser = new UserBuilder().Build();
        var joiningUser = new UserBuilder()
            .WithId(TestConstants.ParticipantUserId)
            .WithFullName("Joining User")
            .WithEmail("joining@example.com")
            .WithPhone("+919876543211")
            .Build();
        _users = [initiatorUser, joiningUser];

        var restaurant = new RestaurantBuilder().Build();

        var groupOrder = new GroupOrderBuilder()
            .WithStatus(GroupOrderStatus.Active)
            .WithInviteCode(TestConstants.ValidInviteCode)
            .Build();
        groupOrder.Restaurant = restaurant;

        var initiatorParticipant = new GroupOrderParticipant
        {
            Id = Guid.NewGuid(),
            GroupOrderId = groupOrder.Id,
            UserId = TestConstants.UserId,
            IsInitiator = true,
            Status = GroupOrderParticipantStatus.Joined,
            JoinedAt = DateTimeOffset.UtcNow,
            User = initiatorUser,
        };
        groupOrder.Participants = new List<GroupOrderParticipant> { initiatorParticipant };

        _groupOrders = [groupOrder];
        _groupOrderParticipants = [initiatorParticipant];

        _db = MockDbContextFactory.Create(
            users: _users,
            restaurants: [restaurant],
            groupOrders: _groupOrders,
            groupOrderParticipants: _groupOrderParticipants);

        _handler = new JoinGroupOrderCommandHandler(_db, _publisher);
    }

    private static JoinGroupOrderCommand ValidCommand() =>
        new(TestConstants.ParticipantUserId, TestConstants.ValidInviteCode);

    [Fact]
    public async Task Handle_ValidInviteCode_JoinsGroup()
    {
        var command = ValidCommand();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.InviteCode.Should().Be(TestConstants.ValidInviteCode);
        result.Value.Participants.Should().Contain(p =>
            p.UserId == TestConstants.ParticipantUserId && !p.IsInitiator);
    }

    [Fact]
    public async Task Handle_InvalidCode_ReturnsFailure()
    {
        var command = new JoinGroupOrderCommand(TestConstants.ParticipantUserId, "XXXXXX");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("GROUP_ORDER_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_GroupOrderExpired_ReturnsFailure()
    {
        _groupOrders[0].ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(-1);

        var command = ValidCommand();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("GROUP_ORDER_EXPIRED");
    }

    [Fact]
    public async Task Handle_AlreadyParticipant_ReturnsFailure()
    {
        var existingParticipant = new GroupOrderParticipant
        {
            Id = Guid.NewGuid(),
            GroupOrderId = _groupOrders[0].Id,
            UserId = TestConstants.ParticipantUserId,
            IsInitiator = false,
            Status = GroupOrderParticipantStatus.Joined,
            JoinedAt = DateTimeOffset.UtcNow,
            User = _users.First(u => u.Id == TestConstants.ParticipantUserId),
        };
        _groupOrders[0].Participants.Add(existingParticipant);
        _groupOrderParticipants.Add(existingParticipant);

        var command = ValidCommand();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("ALREADY_PARTICIPANT");
    }

    [Fact]
    public async Task Handle_UserHasOtherActiveGroupOrder_ReturnsFailure()
    {
        var otherGroupOrder = new GroupOrderBuilder()
            .WithId(Guid.NewGuid())
            .WithInviteCode("B4L8Y3")
            .WithStatus(GroupOrderStatus.Active)
            .Build();
        _groupOrders.Add(otherGroupOrder);

        var otherParticipant = new GroupOrderParticipant
        {
            Id = Guid.NewGuid(),
            GroupOrderId = otherGroupOrder.Id,
            UserId = TestConstants.ParticipantUserId,
            IsInitiator = true,
            Status = GroupOrderParticipantStatus.Joined,
            JoinedAt = DateTimeOffset.UtcNow,
            GroupOrder = otherGroupOrder,
        };
        _groupOrderParticipants.Add(otherParticipant);

        var command = ValidCommand();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("GROUP_ORDER_ALREADY_EXISTS");
    }

    [Fact]
    public async Task Handle_GroupOrderCancelled_ReturnsFailure()
    {
        _groupOrders[0].Status = GroupOrderStatus.Cancelled;

        var command = ValidCommand();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("GROUP_ORDER_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_Success_SavesChanges()
    {
        var command = ValidCommand();

        await _handler.Handle(command, CancellationToken.None);

        await _db.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Success_PublishesNotification()
    {
        var command = ValidCommand();

        await _handler.Handle(command, CancellationToken.None);

        await _publisher.Received(1).Publish(
            Arg.Is<GroupOrderParticipantJoinedNotification>(n =>
                n.GroupOrderId == TestConstants.GroupOrderId
                && n.UserId == TestConstants.ParticipantUserId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UserPreviouslyLeft_CanRejoin()
    {
        var leftParticipant = new GroupOrderParticipant
        {
            Id = Guid.NewGuid(),
            GroupOrderId = _groupOrders[0].Id,
            UserId = TestConstants.ParticipantUserId,
            IsInitiator = false,
            Status = GroupOrderParticipantStatus.Left,
            JoinedAt = DateTimeOffset.UtcNow.AddMinutes(-20),
            LeftAt = DateTimeOffset.UtcNow.AddMinutes(-5),
            User = _users.First(u => u.Id == TestConstants.ParticipantUserId),
        };
        _groupOrders[0].Participants.Add(leftParticipant);
        _groupOrderParticipants.Add(leftParticipant);

        var command = ValidCommand();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
}
