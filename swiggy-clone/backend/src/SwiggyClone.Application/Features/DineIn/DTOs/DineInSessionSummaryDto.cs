using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Application.Features.DineIn.DTOs;

public sealed record DineInSessionSummaryDto(
    Guid Id,
    Guid RestaurantId,
    string RestaurantName,
    string? RestaurantLogoUrl,
    string TableNumber,
    string SessionCode,
    DineInSessionStatus Status,
    int GuestCount,
    DateTimeOffset StartedAt);
