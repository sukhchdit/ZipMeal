namespace SwiggyClone.Application.Features.Banners.DTOs;

public sealed record AdminBannerDto(
    Guid Id,
    string Title,
    string ImageUrl,
    string? DeepLink,
    int DisplayOrder,
    DateTimeOffset ValidFrom,
    DateTimeOffset ValidUntil,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
