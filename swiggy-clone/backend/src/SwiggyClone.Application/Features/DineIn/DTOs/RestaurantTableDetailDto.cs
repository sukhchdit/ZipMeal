using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Application.Features.DineIn.DTOs;

public sealed record RestaurantTableDetailDto(
    Guid Id,
    string TableNumber,
    int Capacity,
    string? FloorSection,
    string QrCodeData,
    TableStatus Status,
    bool IsActive,
    int ActiveSessionCount,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
