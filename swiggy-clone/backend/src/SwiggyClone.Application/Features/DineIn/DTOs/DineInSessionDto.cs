using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Application.Features.DineIn.DTOs;

public sealed record DineInSessionDto(
    Guid Id,
    Guid RestaurantId,
    string RestaurantName,
    string? RestaurantLogoUrl,
    DineInTableDto Table,
    string SessionCode,
    DineInSessionStatus Status,
    int GuestCount,
    DateTimeOffset StartedAt,
    DateTimeOffset? EndedAt,
    List<DineInSessionMemberDto> Members,
    List<DineInOrderSummaryDto> Orders);
