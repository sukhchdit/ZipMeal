using MediatR;
using SwiggyClone.Application.Features.Coupons.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Coupons.Queries;

public sealed record GetCouponsQuery(
    string? Search,
    bool? IsActive,
    int Page,
    int PageSize) : IRequest<Result<PagedResult<AdminCouponDto>>>;
