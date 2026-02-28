using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Application.Features.DineIn.DTOs;

public sealed record DineInOrderSummaryDto(
    Guid Id,
    string OrderNumber,
    Guid PlacedByUserId,
    string PlacedByName,
    OrderStatus Status,
    int TotalAmount,
    int ItemCount,
    DateTimeOffset CreatedAt);
