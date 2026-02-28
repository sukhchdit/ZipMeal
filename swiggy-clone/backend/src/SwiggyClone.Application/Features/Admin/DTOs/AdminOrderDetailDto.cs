using SwiggyClone.Application.Features.Orders.DTOs;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Application.Features.Admin.DTOs;

public sealed record AdminOrderDetailDto(
    Guid Id,
    string OrderNumber,
    Guid UserId,
    string CustomerName,
    string CustomerPhone,
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
    string? CancellationReason,
    DateTimeOffset? EstimatedDeliveryTime,
    DateTimeOffset? ActualDeliveryTime,
    DateTimeOffset CreatedAt,
    List<OrderItemDto> Items);
