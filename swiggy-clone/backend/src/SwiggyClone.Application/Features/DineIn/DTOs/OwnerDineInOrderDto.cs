using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Application.Features.DineIn.DTOs;

public sealed record OwnerDineInOrderDto(
    Guid Id,
    string OrderNumber,
    string TableNumber,
    string CustomerName,
    DineInOrderStatus Status,
    int ItemCount,
    int TotalAmount,
    string? SpecialInstructions,
    DateTimeOffset CreatedAt,
    List<OwnerDineInOrderItemDto> Items);

public sealed record OwnerDineInOrderItemDto(
    string ItemName,
    string? VariantName,
    int Quantity,
    int Price,
    string? SpecialInstructions);
