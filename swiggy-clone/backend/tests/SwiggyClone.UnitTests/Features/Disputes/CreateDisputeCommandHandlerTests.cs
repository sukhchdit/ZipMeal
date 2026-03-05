using FluentAssertions;
using MediatR;
using NSubstitute;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Disputes.Commands.CreateDispute;
using SwiggyClone.Application.Features.Wallet.Commands.CreditWallet;
using SwiggyClone.Application.Features.Wallet.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;
using SwiggyClone.UnitTests.Common;
using SwiggyClone.UnitTests.Common.Builders;

namespace SwiggyClone.UnitTests.Features.Disputes;

public sealed class CreateDisputeCommandHandlerTests
{
    private readonly IAppDbContext _db;
    private readonly ISender _sender;
    private readonly IRealtimeNotifier _notifier;
    private readonly CreateDisputeCommandHandler _handler;
    private readonly List<Order> _orders;
    private readonly List<Dispute> _disputes;
    private readonly List<User> _users;

    public CreateDisputeCommandHandlerTests()
    {
        var user = new UserBuilder().Build();
        _users = [user];
        _orders = [];
        _disputes = [];

        _db = MockDbContextFactory.Create(
            users: _users,
            orders: _orders,
            disputes: _disputes);

        _sender = Substitute.For<ISender>();
        _sender.Send(Arg.Any<CreditWalletCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<WalletTransactionDto>.Success(
                new WalletTransactionDto(Guid.NewGuid(), 0, 0, 0, null, "credit", 0, DateTimeOffset.UtcNow)));

        _notifier = Substitute.For<IRealtimeNotifier>();
        _handler = new CreateDisputeCommandHandler(_db, _sender, _notifier);
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesDispute()
    {
        var order = new OrderBuilder()
            .WithStatus(OrderStatus.Delivered)
            .Build();
        _orders.Add(order);

        var command = new CreateDisputeCommand(
            TestConstants.UserId,
            TestConstants.OrderId,
            (int)DisputeIssueType.LateDelivery,
            "My order arrived very late.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.OrderId.Should().Be(TestConstants.OrderId);
        result.Value.UserId.Should().Be(TestConstants.UserId);
        result.Value.IssueType.Should().Be((int)DisputeIssueType.LateDelivery);
        result.Value.Description.Should().Be("My order arrived very late.");
        result.Value.DisputeNumber.Should().StartWith("DSP-");
        await _db.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _notifier.Received(1).NotifyDisputeEventAsync(
            TestConstants.UserId, Arg.Any<Guid>(), "dispute.created", Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_OrderNotFound_ReturnsFailure()
    {
        // No order in the list — DB will return null
        var command = new CreateDisputeCommand(
            TestConstants.UserId,
            Guid.NewGuid(),
            (int)DisputeIssueType.MissingItems,
            "Missing items in order.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("DISPUTE_ORDER_NOT_ELIGIBLE");
    }

    [Fact]
    public async Task Handle_OrderNotDelivered_ReturnsFailure()
    {
        var order = new OrderBuilder()
            .WithStatus(OrderStatus.Preparing)
            .Build();
        _orders.Add(order);

        var command = new CreateDisputeCommand(
            TestConstants.UserId,
            TestConstants.OrderId,
            (int)DisputeIssueType.WrongItems,
            "Wrong items received.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("DISPUTE_ORDER_NOT_ELIGIBLE");
        result.ErrorMessage.Should().Contain("delivered or cancelled");
    }

    [Fact]
    public async Task Handle_CancelledOrder_CreatesDispute()
    {
        var order = new OrderBuilder()
            .WithStatus(OrderStatus.Cancelled)
            .Build();
        _orders.Add(order);

        var command = new CreateDisputeCommand(
            TestConstants.UserId,
            TestConstants.OrderId,
            (int)DisputeIssueType.NeverDelivered,
            "Order was cancelled without my consent.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_DuplicateActiveDispute_ReturnsFailure()
    {
        var order = new OrderBuilder()
            .WithStatus(OrderStatus.Delivered)
            .Build();
        _orders.Add(order);

        var existingDispute = new DisputeBuilder()
            .WithStatus(DisputeStatus.Opened)
            .Build();
        _disputes.Add(existingDispute);

        var command = new CreateDisputeCommand(
            TestConstants.UserId,
            TestConstants.OrderId,
            (int)DisputeIssueType.MissingItems,
            "Items missing again.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("DISPUTE_ALREADY_EXISTS");
    }

    [Fact]
    public async Task Handle_SmallOrderAutoResolves_CreditsWallet()
    {
        var order = new OrderBuilder()
            .WithStatus(OrderStatus.Delivered)
            .WithTotalAmount(25000) // 250 INR, under 300 INR threshold
            .Build();
        _orders.Add(order);

        var command = new CreateDisputeCommand(
            TestConstants.UserId,
            TestConstants.OrderId,
            (int)DisputeIssueType.MissingItems, // Auto-resolvable issue type
            "Some items were missing.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be((int)DisputeStatus.Resolved);
        result.Value.ResolutionAmountPaise.Should().Be(25000);

        await _sender.Received(1).Send(
            Arg.Is<CreditWalletCommand>(c =>
                c.UserId == TestConstants.UserId &&
                c.AmountPaise == 25000),
            Arg.Any<CancellationToken>());

        await _notifier.Received(1).NotifyDisputeEventAsync(
            TestConstants.UserId, Arg.Any<Guid>(), "dispute.auto_resolved", Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_LargeOrderDoesNotAutoResolve()
    {
        var order = new OrderBuilder()
            .WithStatus(OrderStatus.Delivered)
            .WithTotalAmount(50000) // 500 INR, over 300 INR threshold
            .Build();
        _orders.Add(order);

        var command = new CreateDisputeCommand(
            TestConstants.UserId,
            TestConstants.OrderId,
            (int)DisputeIssueType.MissingItems, // Auto-resolvable issue type, but amount too high
            "Some items were missing.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be((int)DisputeStatus.Opened);

        await _sender.DidNotReceive().Send(
            Arg.Any<CreditWalletCommand>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NonAutoResolvableIssueType_DoesNotAutoResolve()
    {
        var order = new OrderBuilder()
            .WithStatus(OrderStatus.Delivered)
            .WithTotalAmount(10000) // Small amount, but non-auto-resolvable type
            .Build();
        _orders.Add(order);

        var command = new CreateDisputeCommand(
            TestConstants.UserId,
            TestConstants.OrderId,
            (int)DisputeIssueType.LateDelivery, // Not in auto-resolvable list
            "Order was very late.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be((int)DisputeStatus.Opened);

        await _sender.DidNotReceive().Send(
            Arg.Any<CreditWalletCommand>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_OrderBelongsToDifferentUser_ReturnsFailure()
    {
        var otherUserId = Guid.NewGuid();
        var order = new OrderBuilder()
            .WithUserId(otherUserId)
            .WithStatus(OrderStatus.Delivered)
            .Build();
        _orders.Add(order);

        var command = new CreateDisputeCommand(
            TestConstants.UserId,
            TestConstants.OrderId,
            (int)DisputeIssueType.WrongItems,
            "Wrong items.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("DISPUTE_ORDER_NOT_ELIGIBLE");
    }

    [Fact]
    public async Task Handle_RejectedDisputeAllowsNewDispute()
    {
        var order = new OrderBuilder()
            .WithStatus(OrderStatus.Delivered)
            .Build();
        _orders.Add(order);

        var rejectedDispute = new DisputeBuilder()
            .WithStatus(DisputeStatus.Rejected)
            .Build();
        _disputes.Add(rejectedDispute);

        var command = new CreateDisputeCommand(
            TestConstants.UserId,
            TestConstants.OrderId,
            (int)DisputeIssueType.MissingItems,
            "Filing again after rejection.");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
}
