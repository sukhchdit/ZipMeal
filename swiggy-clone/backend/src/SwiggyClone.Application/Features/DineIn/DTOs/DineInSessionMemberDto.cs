namespace SwiggyClone.Application.Features.DineIn.DTOs;

public sealed record DineInSessionMemberDto(
    Guid UserId,
    string FullName,
    string? AvatarUrl,
    short Role,
    DateTimeOffset JoinedAt);
