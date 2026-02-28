using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Application.Features.Admin.DTOs;

public sealed record AdminUserDto(
    Guid Id,
    string FullName,
    string PhoneNumber,
    string? Email,
    string? AvatarUrl,
    UserRole Role,
    bool IsVerified,
    bool IsActive,
    DateTimeOffset? LastLoginAt,
    DateTimeOffset CreatedAt);
