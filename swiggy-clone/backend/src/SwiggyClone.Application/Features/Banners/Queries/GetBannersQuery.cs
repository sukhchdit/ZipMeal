using MediatR;
using SwiggyClone.Application.Features.Banners.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Banners.Queries;

public sealed record GetBannersQuery(
    string? Search,
    bool? IsActive,
    int Page,
    int PageSize) : IRequest<Result<PagedResult<AdminBannerDto>>>;
