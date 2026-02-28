namespace SwiggyClone.Application.Features.Auth.DTOs;

public sealed record SessionDto(
    Guid Id,
    string? DeviceInfo,
    DateTimeOffset CreatedAt,
    DateTimeOffset ExpiresAt,
    bool IsCurrent);
