using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Coupons.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Coupons.Commands;

internal sealed class UpdateCouponCommandHandler(IAppDbContext db)
    : IRequestHandler<UpdateCouponCommand, Result<AdminCouponDto>>
{
    public async Task<Result<AdminCouponDto>> Handle(UpdateCouponCommand request, CancellationToken ct)
    {
        var coupon = await db.Coupons.FirstOrDefaultAsync(c => c.Id == request.Id, ct);
        if (coupon is null)
            return Result<AdminCouponDto>.Failure("COUPON_NOT_FOUND", "Coupon not found.");

        coupon.Title = request.Title;
        coupon.Description = request.Description;
        coupon.MaxDiscount = request.MaxDiscount;
        coupon.MinOrderAmount = request.MinOrderAmount;
        coupon.ValidFrom = request.ValidFrom;
        coupon.ValidUntil = request.ValidUntil;
        coupon.MaxUsages = request.MaxUsages;
        coupon.MaxUsagesPerUser = request.MaxUsagesPerUser;
        coupon.ApplicableOrderTypes = request.ApplicableOrderTypes ?? [];
        coupon.RestaurantIds = request.RestaurantIds;
        coupon.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);

        var dto = new AdminCouponDto(
            coupon.Id, coupon.Code, coupon.Title, coupon.Description,
            (int)coupon.DiscountType, coupon.DiscountValue, coupon.MaxDiscount,
            coupon.MinOrderAmount, coupon.ValidFrom, coupon.ValidUntil,
            coupon.MaxUsages, coupon.MaxUsagesPerUser, coupon.CurrentUsages,
            coupon.ApplicableOrderTypes, coupon.RestaurantIds,
            coupon.IsActive, coupon.CreatedAt, coupon.UpdatedAt);

        return Result<AdminCouponDto>.Success(dto);
    }
}
