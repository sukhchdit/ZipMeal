using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Application.Features.Orders.DTOs;

public sealed record OrderDto(
    Guid Id,
    string OrderNumber,
    Guid RestaurantId,
    string RestaurantName,
    OrderType OrderType,
    OrderStatus Status,
    int Subtotal,
    int TaxAmount,
    int DeliveryFee,
    int PackagingCharge,
    int DiscountAmount,
    int TotalAmount,
    PaymentStatus PaymentStatus,
    PaymentMethod? PaymentMethod,
    string? SpecialInstructions,
    DateTimeOffset? EstimatedDeliveryTime,
    DateTimeOffset? ScheduledDeliveryTime,
    DateTimeOffset CreatedAt,
    List<OrderItemDto> Items,
    bool HasReview,
    int TipAmount,
    bool HasTipped);
