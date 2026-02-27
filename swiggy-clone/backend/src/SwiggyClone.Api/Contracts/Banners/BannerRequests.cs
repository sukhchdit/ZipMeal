namespace SwiggyClone.Api.Contracts.Banners;

public sealed record CreateBannerRequest(
    string Title,
    string ImageUrl,
    string? DeepLink,
    int DisplayOrder,
    DateTimeOffset ValidFrom,
    DateTimeOffset ValidUntil);

public sealed record UpdateBannerRequest(
    string Title,
    string ImageUrl,
    string? DeepLink,
    int DisplayOrder,
    DateTimeOffset ValidFrom,
    DateTimeOffset ValidUntil);

public sealed record ToggleBannerRequest(bool IsActive);
