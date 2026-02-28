namespace SwiggyClone.Application.Features.Coupons.DTOs;

public sealed record CouponDto(
    Guid Id,
    string Code,
    string Title,
    string? Description,
    int DiscountType,
    int DiscountValue,
    int? MaxDiscount,
    int MinOrderAmount,
    DateTimeOffset ValidFrom,
    DateTimeOffset ValidUntil,
    bool IsActive);
