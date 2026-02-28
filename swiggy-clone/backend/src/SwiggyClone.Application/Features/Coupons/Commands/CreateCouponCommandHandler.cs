using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Coupons.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Coupons.Commands;

internal sealed class CreateCouponCommandHandler(IAppDbContext db)
    : IRequestHandler<CreateCouponCommand, Result<AdminCouponDto>>
{
    public async Task<Result<AdminCouponDto>> Handle(CreateCouponCommand request, CancellationToken ct)
    {
        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        var exists = await db.Coupons.AnyAsync(c => c.Code == normalizedCode, ct);
        if (exists)
            return Result<AdminCouponDto>.Failure("COUPON_CODE_EXISTS", "A coupon with this code already exists.");

        var coupon = new Coupon
        {
            Id = Guid.CreateVersion7(),
            Code = normalizedCode,
            Title = request.Title,
            Description = request.Description,
            DiscountType = (DiscountType)request.DiscountType,
            DiscountValue = request.DiscountValue,
            MaxDiscount = request.MaxDiscount,
            MinOrderAmount = request.MinOrderAmount,
            ValidFrom = request.ValidFrom,
            ValidUntil = request.ValidUntil,
            MaxUsages = request.MaxUsages,
            MaxUsagesPerUser = request.MaxUsagesPerUser,
            CurrentUsages = 0,
            ApplicableOrderTypes = request.ApplicableOrderTypes ?? [],
            RestaurantIds = request.RestaurantIds,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        db.Coupons.Add(coupon);
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
