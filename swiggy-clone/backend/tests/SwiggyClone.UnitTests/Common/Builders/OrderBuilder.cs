using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.UnitTests.Common.Builders;

public sealed class OrderBuilder
{
    private Guid _id = TestConstants.OrderId;
    private Guid _userId = TestConstants.UserId;
    private Guid _restaurantId = TestConstants.RestaurantId;
    private OrderStatus _status = OrderStatus.Placed;
    private int _totalAmount = 50000;

    public OrderBuilder WithId(Guid id) { _id = id; return this; }
    public OrderBuilder WithUserId(Guid userId) { _userId = userId; return this; }
    public OrderBuilder WithRestaurantId(Guid restaurantId) { _restaurantId = restaurantId; return this; }
    public OrderBuilder WithStatus(OrderStatus status) { _status = status; return this; }
    public OrderBuilder WithTotalAmount(int amount) { _totalAmount = amount; return this; }

    public Order Build() => new()
    {
        Id = _id,
        OrderNumber = $"ORD-{_id.ToString()[..8].ToUpperInvariant()}",
        UserId = _userId,
        RestaurantId = _restaurantId,
        OrderType = OrderType.Delivery,
        Status = _status,
        Subtotal = _totalAmount - 5000,
        TaxAmount = 2500,
        DeliveryFee = 2500,
        TotalAmount = _totalAmount,
        PaymentStatus = PaymentStatus.Pending,
        DeliveryAddressId = TestConstants.AddressId,
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow,
    };
}
