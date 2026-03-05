using FluentAssertions;
using MediatR;
using NSubstitute;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Tips.Commands;
using SwiggyClone.Application.Features.Wallet.Commands.CreditWallet;
using SwiggyClone.Application.Features.Wallet.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;
using SwiggyClone.UnitTests.Common;
using SwiggyClone.UnitTests.Common.Builders;

namespace SwiggyClone.UnitTests.Features.Tips;

public sealed class SubmitTipCommandHandlerTests
{
    private readonly IAppDbContext _db;
    private readonly ISender _sender;
    private readonly IEventBus _eventBus;
    private readonly SubmitTipCommandHandler _handler;

    private readonly List<Order> _orders = [];
    private readonly List<DeliveryAssignment> _deliveryAssignments = [];

    private static readonly Guid PartnerId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    public SubmitTipCommandHandlerTests()
    {
        _db = MockDbContextFactory.Create(
            orders: _orders,
            deliveryAssignments: _deliveryAssignments);

        _sender = Substitute.For<ISender>();
        _eventBus = Substitute.For<IEventBus>();
        _handler = new SubmitTipCommandHandler(_db, _sender, _eventBus);
    }

    [Fact]
    public async Task Handle_OrderNotFound_ReturnsFailure()
    {
        // Arrange: no orders
        var command = new SubmitTipCommand(TestConstants.UserId, TestConstants.OrderId, 5000);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("ORDER_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_WrongUser_ReturnsFailure()
    {
        // Arrange: order exists but belongs to different user
        var order = new OrderBuilder()
            .WithUserId(Guid.NewGuid())
            .WithStatus(OrderStatus.Delivered)
            .Build();
        order.OrderType = OrderType.Delivery;
        _orders.Add(order);

        var command = new SubmitTipCommand(TestConstants.UserId, TestConstants.OrderId, 5000);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("ORDER_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_OrderNotDelivered_ReturnsFailure()
    {
        // Arrange: order is placed, not yet delivered
        var order = new OrderBuilder()
            .WithStatus(OrderStatus.Placed)
            .Build();
        order.OrderType = OrderType.Delivery;
        _orders.Add(order);

        var command = new SubmitTipCommand(TestConstants.UserId, TestConstants.OrderId, 5000);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("ORDER_NOT_DELIVERED");
    }

    [Fact]
    public async Task Handle_NotDeliveryOrder_ReturnsFailure()
    {
        // Arrange: order is delivered but it's a dine-in order
        var order = new OrderBuilder()
            .WithStatus(OrderStatus.Delivered)
            .Build();
        order.OrderType = OrderType.DineIn;
        _orders.Add(order);

        var command = new SubmitTipCommand(TestConstants.UserId, TestConstants.OrderId, 5000);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("NOT_DELIVERY_ORDER");
    }

    [Fact]
    public async Task Handle_AlreadyTipped_ReturnsFailure()
    {
        // Arrange: order already has a tip
        var order = new OrderBuilder()
            .WithStatus(OrderStatus.Delivered)
            .Build();
        order.OrderType = OrderType.Delivery;
        order.TipAmount = 3000;
        _orders.Add(order);

        var command = new SubmitTipCommand(TestConstants.UserId, TestConstants.OrderId, 5000);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("ALREADY_TIPPED");
    }

    [Fact]
    public async Task Handle_DeliveryAssignmentNotFound_ReturnsFailure()
    {
        // Arrange: order is valid but no delivery assignment found
        var order = new OrderBuilder()
            .WithStatus(OrderStatus.Delivered)
            .Build();
        order.OrderType = OrderType.Delivery;
        order.TipAmount = 0;
        _orders.Add(order);

        var command = new SubmitTipCommand(TestConstants.UserId, TestConstants.OrderId, 5000);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("ASSIGNMENT_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_ValidTip_ReturnsSuccessAndUpdatesTipAmount()
    {
        // Arrange
        var order = new OrderBuilder()
            .WithStatus(OrderStatus.Delivered)
            .Build();
        order.OrderType = OrderType.Delivery;
        order.TipAmount = 0;
        _orders.Add(order);

        _deliveryAssignments.Add(new DeliveryAssignment
        {
            Id = Guid.NewGuid(),
            OrderId = TestConstants.OrderId,
            PartnerId = PartnerId,
            Status = DeliveryStatus.Delivered,
            AssignedAt = DateTimeOffset.UtcNow.AddHours(-1),
            CreatedAt = DateTimeOffset.UtcNow.AddHours(-1),
            UpdatedAt = DateTimeOffset.UtcNow,
        });

        var txnDto = new WalletTransactionDto(
            Guid.NewGuid(), 5000, 0, 0, null, "Tip", 5000, DateTimeOffset.UtcNow);
        _sender.Send(Arg.Any<CreditWalletCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<WalletTransactionDto>.Success(txnDto));

        var command = new SubmitTipCommand(TestConstants.UserId, TestConstants.OrderId, 5000);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.TipAmount.Should().Be(5000);
        await _db.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidTip_CreditsPartnerWallet()
    {
        // Arrange
        var order = new OrderBuilder()
            .WithStatus(OrderStatus.Delivered)
            .Build();
        order.OrderType = OrderType.Delivery;
        order.TipAmount = 0;
        _orders.Add(order);

        _deliveryAssignments.Add(new DeliveryAssignment
        {
            Id = Guid.NewGuid(),
            OrderId = TestConstants.OrderId,
            PartnerId = PartnerId,
            Status = DeliveryStatus.Delivered,
            AssignedAt = DateTimeOffset.UtcNow.AddHours(-1),
            CreatedAt = DateTimeOffset.UtcNow.AddHours(-1),
            UpdatedAt = DateTimeOffset.UtcNow,
        });

        var txnDto = new WalletTransactionDto(
            Guid.NewGuid(), 5000, 0, 0, null, "Tip", 5000, DateTimeOffset.UtcNow);
        _sender.Send(Arg.Any<CreditWalletCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<WalletTransactionDto>.Success(txnDto));

        var command = new SubmitTipCommand(TestConstants.UserId, TestConstants.OrderId, 5000);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _sender.Received(1).Send(
            Arg.Is<CreditWalletCommand>(c =>
                c.UserId == PartnerId &&
                c.AmountPaise == 5000),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidTip_PublishesNotificationViaEventBus()
    {
        // Arrange
        var order = new OrderBuilder()
            .WithStatus(OrderStatus.Delivered)
            .Build();
        order.OrderType = OrderType.Delivery;
        order.TipAmount = 0;
        _orders.Add(order);

        _deliveryAssignments.Add(new DeliveryAssignment
        {
            Id = Guid.NewGuid(),
            OrderId = TestConstants.OrderId,
            PartnerId = PartnerId,
            Status = DeliveryStatus.Delivered,
            AssignedAt = DateTimeOffset.UtcNow.AddHours(-1),
            CreatedAt = DateTimeOffset.UtcNow.AddHours(-1),
            UpdatedAt = DateTimeOffset.UtcNow,
        });

        var txnDto = new WalletTransactionDto(
            Guid.NewGuid(), 5000, 0, 0, null, "Tip", 5000, DateTimeOffset.UtcNow);
        _sender.Send(Arg.Any<CreditWalletCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<WalletTransactionDto>.Success(txnDto));

        var command = new SubmitTipCommand(TestConstants.UserId, TestConstants.OrderId, 5000);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _eventBus.Received(1).PublishAsync(
            Arg.Any<string>(),
            PartnerId.ToString(),
            Arg.Any<object>(),
            Arg.Any<CancellationToken>());
    }
}
