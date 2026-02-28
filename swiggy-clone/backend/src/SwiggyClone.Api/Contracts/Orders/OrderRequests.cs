using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Api.Contracts.Orders;

public sealed record PlaceOrderRequest(
    Guid DeliveryAddressId,
    int PaymentMethod,
    string? SpecialInstructions,
    string? CouponCode);

public sealed record CancelOrderRequest(string? CancellationReason);

public sealed record UpdateOrderStatusRequest(
    OrderStatus NewStatus,
    string? Notes);
