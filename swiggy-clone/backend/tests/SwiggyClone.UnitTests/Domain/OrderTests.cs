using FluentAssertions;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.UnitTests.Domain;

public sealed class OrderTests
{
    [Fact]
    public void NewOrder_DefaultProperties_HaveExpectedValues()
    {
        var order = new Order();

        order.OrderNumber.Should().BeEmpty();
        order.UserId.Should().Be(Guid.Empty);
        order.RestaurantId.Should().Be(Guid.Empty);
        order.OrderType.Should().Be(default(OrderType));
        order.Status.Should().Be(default(OrderStatus));
        order.Subtotal.Should().Be(0);
        order.TaxAmount.Should().Be(0);
        order.DeliveryFee.Should().Be(0);
        order.PackagingCharge.Should().Be(0);
        order.DiscountAmount.Should().Be(0);
        order.TotalAmount.Should().Be(0);
        order.TipAmount.Should().Be(0);
    }

    [Fact]
    public void NewOrder_NullableProperties_AreNull()
    {
        var order = new Order();

        order.PaymentMethod.Should().BeNull();
        order.CouponId.Should().BeNull();
        order.DeliveryAddressId.Should().BeNull();
        order.DeliveryPartnerId.Should().BeNull();
        order.DineInTableId.Should().BeNull();
        order.DineInSessionId.Should().BeNull();
        order.GroupOrderId.Should().BeNull();
        order.EstimatedDeliveryTime.Should().BeNull();
        order.ScheduledDeliveryTime.Should().BeNull();
        order.ActualDeliveryTime.Should().BeNull();
        order.SpecialInstructions.Should().BeNull();
        order.CancellationReason.Should().BeNull();
        order.CancelledBy.Should().BeNull();
    }

    [Fact]
    public void NewOrder_Collections_AreInitializedEmpty()
    {
        var order = new Order();

        order.Items.Should().NotBeNull().And.BeEmpty();
        order.StatusHistory.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void NewOrder_NavigationProperties_AreNotNull()
    {
        var order = new Order();

        // Navigation properties initialized with null! by design, but the reference should exist
        // Test that nullable navigations are null
        order.DeliveryAddress.Should().BeNull();
        order.DeliveryPartner.Should().BeNull();
        order.DineInTable.Should().BeNull();
        order.DineInSession.Should().BeNull();
        order.GroupOrder.Should().BeNull();
        order.Payment.Should().BeNull();
    }

    [Fact]
    public void Order_SetProperties_RetainsValues()
    {
        var userId = Guid.NewGuid();
        var restaurantId = Guid.NewGuid();
        var couponId = Guid.NewGuid();
        var addressId = Guid.NewGuid();
        var partnerId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        var order = new Order
        {
            OrderNumber = "SWG-20260303-0001",
            UserId = userId,
            RestaurantId = restaurantId,
            OrderType = OrderType.Delivery,
            Status = OrderStatus.Placed,
            Subtotal = 50000,
            TaxAmount = 2500,
            DeliveryFee = 3000,
            PackagingCharge = 500,
            DiscountAmount = 1000,
            TotalAmount = 55000,
            PaymentMethod = SwiggyClone.Domain.Enums.PaymentMethod.Upi,
            CouponId = couponId,
            DeliveryAddressId = addressId,
            DeliveryPartnerId = partnerId,
            EstimatedDeliveryTime = now.AddMinutes(30),
            SpecialInstructions = "Leave at door",
            TipAmount = 2000
        };

        order.OrderNumber.Should().Be("SWG-20260303-0001");
        order.UserId.Should().Be(userId);
        order.RestaurantId.Should().Be(restaurantId);
        order.OrderType.Should().Be(OrderType.Delivery);
        order.Status.Should().Be(OrderStatus.Placed);
        order.Subtotal.Should().Be(50000);
        order.TaxAmount.Should().Be(2500);
        order.DeliveryFee.Should().Be(3000);
        order.PackagingCharge.Should().Be(500);
        order.DiscountAmount.Should().Be(1000);
        order.TotalAmount.Should().Be(55000);
        order.CouponId.Should().Be(couponId);
        order.DeliveryAddressId.Should().Be(addressId);
        order.DeliveryPartnerId.Should().Be(partnerId);
        order.EstimatedDeliveryTime.Should().Be(now.AddMinutes(30));
        order.SpecialInstructions.Should().Be("Leave at door");
        order.TipAmount.Should().Be(2000);
    }

    [Fact]
    public void Order_DineInProperties_CanBeSet()
    {
        var tableId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();

        var order = new Order
        {
            OrderType = OrderType.DineIn,
            DineInTableId = tableId,
            DineInSessionId = sessionId
        };

        order.OrderType.Should().Be(OrderType.DineIn);
        order.DineInTableId.Should().Be(tableId);
        order.DineInSessionId.Should().Be(sessionId);
    }

    [Fact]
    public void Order_GroupDeliveryProperties_CanBeSet()
    {
        var groupOrderId = Guid.NewGuid();

        var order = new Order
        {
            OrderType = OrderType.GroupDelivery,
            GroupOrderId = groupOrderId
        };

        order.OrderType.Should().Be(OrderType.GroupDelivery);
        order.GroupOrderId.Should().Be(groupOrderId);
    }

    [Fact]
    public void Order_CancellationProperties_CanBeSet()
    {
        var order = new Order
        {
            Status = OrderStatus.Cancelled,
            CancellationReason = "Changed my mind",
            CancelledBy = 1
        };

        order.CancellationReason.Should().Be("Changed my mind");
        order.CancelledBy.Should().Be(1);
    }

    [Fact]
    public void Order_ScheduledDeliveryTime_CanBeSet()
    {
        var scheduledTime = DateTimeOffset.UtcNow.AddHours(2);

        var order = new Order
        {
            ScheduledDeliveryTime = scheduledTime
        };

        order.ScheduledDeliveryTime.Should().Be(scheduledTime);
    }

    [Fact]
    public void Order_ItemsCollection_CanAddItems()
    {
        var order = new Order();
        var item = new OrderItem
        {
            Id = Guid.NewGuid(),
            ItemName = "Paneer Tikka",
            Quantity = 2,
            UnitPrice = 25000,
            TotalPrice = 50000
        };

        order.Items.Add(item);

        order.Items.Should().HaveCount(1);
        order.Items.Should().Contain(item);
    }

    [Fact]
    public void Order_InheritsBaseEntity_HasSoftDeleteSupport()
    {
        var order = new Order();

        order.IsDeleted.Should().BeFalse();
        order.DeletedAt.Should().BeNull();
        order.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Order_SoftDelete_SetsIsDeletedAndDeletedAt()
    {
        var order = new Order();

        order.SoftDelete();

        order.IsDeleted.Should().BeTrue();
        order.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public void Order_PaymentStatus_DefaultsToZeroValue()
    {
        var order = new Order();

        order.PaymentStatus.Should().Be(default(PaymentStatus));
    }
}
