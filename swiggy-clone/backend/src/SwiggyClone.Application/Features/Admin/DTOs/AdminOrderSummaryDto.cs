using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Application.Features.Admin.DTOs;

public sealed record AdminOrderSummaryDto(
    Guid Id,
    string OrderNumber,
    string CustomerName,
    string RestaurantName,
    OrderStatus Status,
    PaymentStatus PaymentStatus,
    int TotalAmount,
    DateTimeOffset CreatedAt);
