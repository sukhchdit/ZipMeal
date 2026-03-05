using FluentAssertions;
using MediatR;
using NSubstitute;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Orders.Commands;
using SwiggyClone.Application.Features.Orders.Notifications;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.UnitTests.Common;
using SwiggyClone.UnitTests.Common.Builders;

namespace SwiggyClone.UnitTests.Features.Orders;

public sealed class CancelOrderCommandHandlerTests
{
    private readonly IAppDbContext _db;
    private readonly IPaymentGatewayService _paymentGateway;
    private readonly IPublisher _publisher;
    private readonly CancelOrderCommandHandler _handler;

    private readonly List<Order> _orders = [];
    private readonly List<OrderStatusHistory> _orderStatusHistory = [];

    public CancelOrderCommandHandlerTests()
    {
        _db = MockDbContextFactory.Create(
            orders: _orders,
            orderStatusHistory: _orderStatusHistory);

        _paymentGateway = Substitute.For<IPaymentGatewayService>();
        _publisher = Substitute.For<IPublisher>();
        _handler = new CancelOrderCommandHandler(_db, _paymentGateway, _publisher);
    }

    [Fact]
    public async Task Handle_OrderNotFound_ReturnsFailure()
    {
        // Arrange: no orders in DB
        var command = new CancelOrderCommand(TestConstants.UserId, TestConstants.OrderId, "Changed my mind");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("ORDER_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_WrongUser_ReturnsFailure()
    {
        // Arrange: order exists but belongs to a different user
        var differentUserId = Guid.NewGuid();
        var order = new OrderBuilder()
            .WithUserId(differentUserId)
            .WithStatus(OrderStatus.Placed)
            .Build();
        _orders.Add(order);

        var command = new CancelOrderCommand(TestConstants.UserId, TestConstants.OrderId, "Changed my mind");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("ORDER_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_AlreadyDelivered_ReturnsFailure()
    {
        // Arrange: order is already delivered
        var order = new OrderBuilder()
            .WithStatus(OrderStatus.Delivered)
            .Build();
        _orders.Add(order);

        var command = new CancelOrderCommand(TestConstants.UserId, TestConstants.OrderId, "Want refund");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("CANCELLATION_NOT_ALLOWED");
    }

    [Fact]
    public async Task Handle_OrderPreparing_ReturnsFailure()
    {
        // Arrange: order is in Preparing status (not Placed/Confirmed)
        var order = new OrderBuilder()
            .WithStatus(OrderStatus.Preparing)
            .Build();
        _orders.Add(order);

        var command = new CancelOrderCommand(TestConstants.UserId, TestConstants.OrderId, "Changed my mind");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("CANCELLATION_NOT_ALLOWED");
    }

    [Fact]
    public async Task Handle_ValidCancelPlacedOrder_ReturnsSuccess()
    {
        // Arrange
        var order = new OrderBuilder()
            .WithStatus(OrderStatus.Placed)
            .Build();
        _orders.Add(order);

        var command = new CancelOrderCommand(TestConstants.UserId, TestConstants.OrderId, "Changed my mind");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Cancelled);
        order.CancellationReason.Should().Be("Changed my mind");
        order.CancelledBy.Should().Be(1);
        _orderStatusHistory.Should().HaveCount(1);
        await _db.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidCancelConfirmedOrder_ReturnsSuccess()
    {
        // Arrange: Confirmed status is also cancellable
        var order = new OrderBuilder()
            .WithStatus(OrderStatus.Confirmed)
            .Build();
        _orders.Add(order);

        var command = new CancelOrderCommand(TestConstants.UserId, TestConstants.OrderId, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public async Task Handle_ValidCancel_PublishesOrderCancelledNotification()
    {
        // Arrange
        var order = new OrderBuilder()
            .WithStatus(OrderStatus.Placed)
            .Build();
        _orders.Add(order);

        var command = new CancelOrderCommand(TestConstants.UserId, TestConstants.OrderId, "No longer needed");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _publisher.Received(1).Publish(
            Arg.Any<OrderCancelledNotification>(), Arg.Any<CancellationToken>());
    }
}
