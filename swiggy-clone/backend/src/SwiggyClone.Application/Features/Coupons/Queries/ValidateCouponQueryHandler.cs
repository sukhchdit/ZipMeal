using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Coupons.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Coupons.Queries;

internal sealed class ValidateCouponQueryHandler(IAppDbContext db)
    : IRequestHandler<ValidateCouponQuery, Result<CouponValidationDto>>
{
    public async Task<Result<CouponValidationDto>> Handle(ValidateCouponQuery request, CancellationToken ct)
    {
        var code = request.Code.Trim().ToUpperInvariant();

        var coupon = await db.Coupons
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == code, ct);

        if (coupon is null)
            return InvalidResult(code, "Coupon code not found.");

        if (!coupon.IsActive)
            return InvalidResult(code, "This coupon is no longer active.");

        var now = DateTimeOffset.UtcNow;
        if (now < coupon.ValidFrom)
            return InvalidResult(code, "This coupon is not yet valid.");
        if (now > coupon.ValidUntil)
            return InvalidResult(code, "This coupon has expired.");

        if (coupon.MaxUsages.HasValue && coupon.CurrentUsages >= coupon.MaxUsages.Value)
            return InvalidResult(code, "This coupon has reached its usage limit.");

        if (request.OrderSubtotal < coupon.MinOrderAmount)
            return InvalidResult(code, $"Minimum order amount is \u20B9{coupon.MinOrderAmount / 100}.");

        if (coupon.ApplicableOrderTypes.Length > 0 &&
            !coupon.ApplicableOrderTypes.Contains((short)request.OrderType))
            return InvalidResult(code, "This coupon is not valid for this order type.");

        if (coupon.RestaurantIds is not null && coupon.RestaurantIds.Length > 0 &&
            !coupon.RestaurantIds.Contains(request.RestaurantId))
            return InvalidResult(code, "This coupon is not valid for this restaurant.");

        // Per-user usage check
        var userUsageCount = await db.CouponUsages
            .CountAsync(u => u.CouponId == coupon.Id && u.UserId == request.UserId, ct);
        if (userUsageCount >= coupon.MaxUsagesPerUser)
            return InvalidResult(code, "You have already used this coupon the maximum number of times.");

        // Calculate discount
        int discountAmount;
        if (coupon.DiscountType == DiscountType.Percentage)
        {
            discountAmount = (int)(request.OrderSubtotal * (coupon.DiscountValue / 100.0));
            if (coupon.MaxDiscount.HasValue && discountAmount > coupon.MaxDiscount.Value)
                discountAmount = coupon.MaxDiscount.Value;
        }
        else
        {
            discountAmount = Math.Min(coupon.DiscountValue, request.OrderSubtotal);
        }

        var dto = new CouponValidationDto(
            coupon.Id, coupon.Code, coupon.Title, coupon.Description,
            discountAmount, true, null);

        return Result<CouponValidationDto>.Success(dto);
    }

    private static Result<CouponValidationDto> InvalidResult(string code, string reason)
    {
        var dto = new CouponValidationDto(Guid.Empty, code, string.Empty, null, 0, false, reason);
        return Result<CouponValidationDto>.Success(dto);
    }
}
