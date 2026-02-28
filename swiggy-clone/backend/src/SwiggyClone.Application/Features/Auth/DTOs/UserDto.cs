namespace SwiggyClone.Application.Features.Auth.DTOs;

public sealed record UserDto(
    Guid Id,
    string PhoneNumber,
    string? Email,
    string FullName,
    string? AvatarUrl,
    string Role,
    bool IsVerified,
    DateTimeOffset? LastLoginAt);
