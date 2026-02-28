using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Application.Features.DineIn.DTOs;

public sealed record OwnerSessionDto(
    Guid Id,
    string TableNumber,
    string? FloorSection,
    string SessionCode,
    DineInSessionStatus Status,
    int GuestCount,
    int MemberCount,
    int OrderCount,
    int TotalAmount,
    DateTimeOffset StartedAt,
    DateTimeOffset? EndedAt);
