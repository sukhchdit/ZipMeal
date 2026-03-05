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

public sealed class SetParticipantReadyCommandHandlerTests
{
    private readonly IPublisher _publisher;
    private readonly List<GroupOrder> _groupOrders;
    private readonly List<GroupOrderParticipant> _groupOrderParticipants;
    private readonly IAppDbContext _db;
    private readonly SetParticipantReadyCommandHandler _handler;

    public SetParticipantReadyCommandHandlerTests()
    {
        _publisher = Substitute.For<IPublisher>();

        var groupOrder = new GroupOrderBuilder()
            .WithStatus(GroupOrderStatus.Active)
            .Build();

        var participant = new GroupOrderParticipant
        {
            Id = Guid.NewGuid(),
            GroupOrderId = groupOrder.Id,
            UserId = TestConstants.ParticipantUserId,
            IsInitiator = false,
            Status = GroupOrderParticipantStatus.Joined,
            JoinedAt = DateTimeOffset.UtcNow,
        };

        _groupOrders = [groupOrder];
        _groupOrderParticipants = [participant];

        _db = MockDbContextFactory.Create(
            groupOrders: _groupOrders,
            groupOrderParticipants: _groupOrderParticipants);

        _handler = new SetParticipantReadyCommandHandler(_db, _publisher);
    }

    [Fact]
    public async Task Handle_JoinedParticipant_SetsReady()
    {
        var command = new SetParticipantReadyCommand(TestConstants.ParticipantUserId, TestConstants.GroupOrderId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _groupOrderParticipants[0].Status.Should().Be(GroupOrderParticipantStatus.Ready);
    }

    [Fact]
    public async Task Handle_GroupOrderNotActive_ReturnsFailure()
    {
        _groupOrders[0].Status = GroupOrderStatus.Cancelled;

        var command = new SetParticipantReadyCommand(TestConstants.ParticipantUserId, TestConstants.GroupOrderId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("GROUP_ORDER_NOT_ACTIVE");
    }

    [Fact]
    public async Task Handle_GroupOrderNotFound_ReturnsFailure()
    {
        var command = new SetParticipantReadyCommand(TestConstants.ParticipantUserId, Guid.NewGuid());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("GROUP_ORDER_NOT_ACTIVE");
    }

    [Fact]
    public async Task Handle_NotParticipant_ReturnsFailure()
    {
        var command = new SetParticipantReadyCommand(Guid.NewGuid(), TestConstants.GroupOrderId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("NOT_PARTICIPANT");
    }

    [Fact]
    public async Task Handle_Success_SavesChanges()
    {
        var command = new SetParticipantReadyCommand(TestConstants.ParticipantUserId, TestConstants.GroupOrderId);

        await _handler.Handle(command, CancellationToken.None);

        await _db.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Success_PublishesNotification()
    {
        var command = new SetParticipantReadyCommand(TestConstants.ParticipantUserId, TestConstants.GroupOrderId);

        await _handler.Handle(command, CancellationToken.None);

        await _publisher.Received(1).Publish(
            Arg.Is<GroupOrderParticipantReadyNotification>(n =>
                n.GroupOrderId == TestConstants.GroupOrderId
                && n.UserId == TestConstants.ParticipantUserId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Success_UpdatesGroupOrderTimestamp()
    {
        var beforeUpdate = _groupOrders[0].UpdatedAt;
        var command = new SetParticipantReadyCommand(TestConstants.ParticipantUserId, TestConstants.GroupOrderId);

        await _handler.Handle(command, CancellationToken.None);

        _groupOrders[0].UpdatedAt.Should().BeOnOrAfter(beforeUpdate);
    }

    [Fact]
    public async Task Handle_ParticipantAlreadyLeft_ReturnsFailure()
    {
        _groupOrderParticipants[0].Status = GroupOrderParticipantStatus.Left;

        var command = new SetParticipantReadyCommand(TestConstants.ParticipantUserId, TestConstants.GroupOrderId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("NOT_PARTICIPANT");
    }
}
