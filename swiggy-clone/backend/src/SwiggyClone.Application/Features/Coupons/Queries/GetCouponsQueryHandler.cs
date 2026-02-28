using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Coupons.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Coupons.Queries;

internal sealed class GetCouponsQueryHandler(IAppDbContext db)
    : IRequestHandler<GetCouponsQuery, Result<PagedResult<AdminCouponDto>>>
{
    public async Task<Result<PagedResult<AdminCouponDto>>> Handle(GetCouponsQuery request, CancellationToken ct)
    {
        var query = db.Coupons.AsNoTracking().AsQueryable();

        if (request.IsActive.HasValue)
            query = query.Where(c => c.IsActive == request.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = $"%{request.Search}%";
            query = query.Where(c =>
                EF.Functions.Like(c.Code, search) ||
                EF.Functions.Like(c.Title, search));
        }

        query = query.OrderByDescending(c => c.CreatedAt);

        var totalCount = await query.CountAsync(ct);

        var coupons = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new AdminCouponDto(
                c.Id,
                c.Code,
                c.Title,
                c.Description,
                (int)c.DiscountType,
                c.DiscountValue,
                c.MaxDiscount,
                c.MinOrderAmount,
                c.ValidFrom,
                c.ValidUntil,
                c.MaxUsages,
                c.MaxUsagesPerUser,
                c.CurrentUsages,
                c.ApplicableOrderTypes,
                c.RestaurantIds,
                c.IsActive,
                c.CreatedAt,
                c.UpdatedAt))
            .ToListAsync(ct);

        var result = new PagedResult<AdminCouponDto>(coupons, totalCount, request.Page, request.PageSize);
        return Result<PagedResult<AdminCouponDto>>.Success(result);
    }
}
