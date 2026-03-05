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

public sealed class CancelGroupOrderCommandHandlerTests
{
    private readonly IPublisher _publisher;
    private readonly IGroupCartService _groupCartService;
    private readonly List<GroupOrder> _groupOrders;
    private readonly IAppDbContext _db;
    private readonly CancelGroupOrderCommandHandler _handler;

    public CancelGroupOrderCommandHandlerTests()
    {
        _publisher = Substitute.For<IPublisher>();
        _groupCartService = Substitute.For<IGroupCartService>();
        _groupCartService.ClearAllCartsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var groupOrder = new GroupOrderBuilder()
            .WithStatus(GroupOrderStatus.Active)
            .WithInitiatorUserId(TestConstants.UserId)
            .Build();

        _groupOrders = [groupOrder];

        _db = MockDbContextFactory.Create(groupOrders: _groupOrders);

        _handler = new CancelGroupOrderCommandHandler(_db, _groupCartService, _publisher);
    }

    [Fact]
    public async Task Handle_Initiator_CancelsGroupOrder()
    {
        var command = new CancelGroupOrderCommand(TestConstants.UserId, TestConstants.GroupOrderId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _groupOrders[0].Status.Should().Be(GroupOrderStatus.Cancelled);
    }

    [Fact]
    public async Task Handle_NotInitiator_ReturnsFailure()
    {
        var command = new CancelGroupOrderCommand(TestConstants.ParticipantUserId, TestConstants.GroupOrderId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("NOT_INITIATOR");
    }

    [Fact]
    public async Task Handle_GroupOrderNotActive_ReturnsFailure()
    {
        _groupOrders[0].Status = GroupOrderStatus.Cancelled;

        var command = new CancelGroupOrderCommand(TestConstants.UserId, TestConstants.GroupOrderId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("GROUP_ORDER_NOT_ACTIVE");
    }

    [Fact]
    public async Task Handle_GroupOrderNotFound_ReturnsFailure()
    {
        var command = new CancelGroupOrderCommand(TestConstants.UserId, Guid.NewGuid());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("GROUP_ORDER_NOT_ACTIVE");
    }

    [Fact]
    public async Task Handle_Success_ClearsAllCarts()
    {
        var command = new CancelGroupOrderCommand(TestConstants.UserId, TestConstants.GroupOrderId);

        await _handler.Handle(command, CancellationToken.None);

        await _groupCartService.Received(1).ClearAllCartsAsync(
            TestConstants.GroupOrderId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Success_SavesChanges()
    {
        var command = new CancelGroupOrderCommand(TestConstants.UserId, TestConstants.GroupOrderId);

        await _handler.Handle(command, CancellationToken.None);

        await _db.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Success_PublishesNotification()
    {
        var command = new CancelGroupOrderCommand(TestConstants.UserId, TestConstants.GroupOrderId);

        await _handler.Handle(command, CancellationToken.None);

        await _publisher.Received(1).Publish(
            Arg.Is<GroupOrderCancelledNotification>(n =>
                n.GroupOrderId == TestConstants.GroupOrderId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Success_UpdatesTimestamp()
    {
        var beforeUpdate = _groupOrders[0].UpdatedAt;
        var command = new CancelGroupOrderCommand(TestConstants.UserId, TestConstants.GroupOrderId);

        await _handler.Handle(command, CancellationToken.None);

        _groupOrders[0].UpdatedAt.Should().BeOnOrAfter(beforeUpdate);
    }

    [Fact]
    public async Task Handle_Failure_DoesNotClearCarts()
    {
        var command = new CancelGroupOrderCommand(TestConstants.ParticipantUserId, TestConstants.GroupOrderId);

        await _handler.Handle(command, CancellationToken.None);

        await _groupCartService.DidNotReceive().ClearAllCartsAsync(
            Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ExpiredGroupOrder_ReturnsFailure()
    {
        _groupOrders[0].Status = GroupOrderStatus.Expired;

        var command = new CancelGroupOrderCommand(TestConstants.UserId, TestConstants.GroupOrderId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("GROUP_ORDER_NOT_ACTIVE");
    }
}
