using MediatR;
using SwiggyClone.Application.Features.Banners.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Banners.Commands;

public sealed record UpdateBannerCommand(
    Guid Id,
    string Title,
    string ImageUrl,
    string? DeepLink,
    int DisplayOrder,
    DateTimeOffset ValidFrom,
    DateTimeOffset ValidUntil) : IRequest<Result<AdminBannerDto>>;
