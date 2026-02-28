using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Application.Features.Orders.DTOs;

public sealed record OrderSummaryDto(
    Guid Id,
    string OrderNumber,
    string RestaurantName,
    string? RestaurantLogoUrl,
    OrderStatus Status,
    int TotalAmount,
    int ItemCount,
    DateTimeOffset CreatedAt);
