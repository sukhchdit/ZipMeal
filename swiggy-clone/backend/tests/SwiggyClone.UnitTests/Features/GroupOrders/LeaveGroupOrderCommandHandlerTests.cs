using FluentAssertions;
using MediatR;
using NSubstitute;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.GroupOrders.Commands;
using SwiggyClone.Application.Features.GroupOrders.Notifications;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;
using SwiggyClone.UnitTests.Common;
using SwiggyClone.UnitTests.Common.Builders;

namespace SwiggyClone.UnitTests.Features.GroupOrders;

public sealed class LeaveGroupOrderCommandHandlerTests
{
    private readonly IPublisher _publisher;
    private readonly IGroupCartService _groupCartService;
    private readonly List<GroupOrder> _groupOrders;
    private readonly List<GroupOrderParticipant> _groupOrderParticipants;
    private readonly IAppDbContext _db;
    private readonly LeaveGroupOrderCommandHandler _handler;

    public LeaveGroupOrderCommandHandlerTests()
    {
        _publisher = Substitute.For<IPublisher>();
        _groupCartService = Substitute.For<IGroupCartService>();
        _groupCartService.ClearParticipantCartAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var groupOrder = new GroupOrderBuilder()
            .WithStatus(GroupOrderStatus.Active)
            .Build();

        var initiatorParticipant = new GroupOrderParticipant
        {
            Id = Guid.NewGuid(),
            GroupOrderId = groupOrder.Id,
            UserId = TestConstants.UserId,
            IsInitiator = true,
            Status = GroupOrderParticipantStatus.Joined,
            JoinedAt = DateTimeOffset.UtcNow,
        };

        var regularParticipant = new GroupOrderParticipant
        {
            Id = Guid.NewGuid(),
            GroupOrderId = groupOrder.Id,
            UserId = TestConstants.ParticipantUserId,
            IsInitiator = false,
            Status = GroupOrderParticipantStatus.Joined,
            JoinedAt = DateTimeOffset.UtcNow,
        };

        _groupOrders = [groupOrder];
        _groupOrderParticipants = [initiatorParticipant, regularParticipant];

        _db = MockDbContextFactory.Create(
            groupOrders: _groupOrders,
            groupOrderParticipants: _groupOrderParticipants);

        _handler = new LeaveGroupOrderCommandHandler(_db, _groupCartService, _publisher);
    }

    [Fact]
    public async Task Handle_Participant_LeavesGroup()
    {
        var command = new LeaveGroupOrderCommand(TestConstants.ParticipantUserId, TestConstants.GroupOrderId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var participant = _groupOrderParticipants.First(p => p.UserId == TestConstants.ParticipantUserId);
        participant.Status.Should().Be(GroupOrderParticipantStatus.Left);
        participant.LeftAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Initiator_ReturnsFailure()
    {
        var command = new LeaveGroupOrderCommand(TestConstants.UserId, TestConstants.GroupOrderId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("INITIATOR_CANNOT_LEAVE");
    }

    [Fact]
    public async Task Handle_NotParticipant_ReturnsFailure()
    {
        var nonParticipantId = Guid.NewGuid();
        var command = new LeaveGroupOrderCommand(nonParticipantId, TestConstants.GroupOrderId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("NOT_PARTICIPANT");
    }

    [Fact]
    public async Task Handle_GroupOrderNotFound_ReturnsFailure()
    {
        var command = new LeaveGroupOrderCommand(TestConstants.ParticipantUserId, Guid.NewGuid());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("GROUP_ORDER_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_GroupOrderNotActive_ReturnsFailure()
    {
        _groupOrders[0].Status = GroupOrderStatus.Cancelled;

        var command = new LeaveGroupOrderCommand(TestConstants.ParticipantUserId, TestConstants.GroupOrderId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("GROUP_ORDER_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_Success_ClearsParticipantCart()
    {
        var command = new LeaveGroupOrderCommand(TestConstants.ParticipantUserId, TestConstants.GroupOrderId);

        await _handler.Handle(command, CancellationToken.None);

        await _groupCartService.Received(1).ClearParticipantCartAsync(
            TestConstants.GroupOrderId, TestConstants.ParticipantUserId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Success_SavesChanges()
    {
        var command = new LeaveGroupOrderCommand(TestConstants.ParticipantUserId, TestConstants.GroupOrderId);

        await _handler.Handle(command, CancellationToken.None);

        await _db.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Success_PublishesNotification()
    {
        var command = new LeaveGroupOrderCommand(TestConstants.ParticipantUserId, TestConstants.GroupOrderId);

        await _handler.Handle(command, CancellationToken.None);

        await _publisher.Received(1).Publish(
            Arg.Is<GroupOrderParticipantLeftNotification>(n =>
                n.GroupOrderId == TestConstants.GroupOrderId
                && n.UserId == TestConstants.ParticipantUserId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_AlreadyLeftParticipant_ReturnsFailure()
    {
        var leftParticipant = _groupOrderParticipants.First(p => p.UserId == TestConstants.ParticipantUserId);
        leftParticipant.Status = GroupOrderParticipantStatus.Left;
        leftParticipant.LeftAt = DateTimeOffset.UtcNow.AddMinutes(-5);

        var command = new LeaveGroupOrderCommand(TestConstants.ParticipantUserId, TestConstants.GroupOrderId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("NOT_PARTICIPANT");
    }
}
