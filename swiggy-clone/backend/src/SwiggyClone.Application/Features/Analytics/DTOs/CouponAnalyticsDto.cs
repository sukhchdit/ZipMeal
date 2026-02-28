namespace SwiggyClone.Application.Features.Analytics.DTOs;

public sealed record CouponAnalyticsDto(
    int TotalCouponsUsed,
    long TotalDiscountGiven,
    int UniqueCouponsUsed,
    List<NamedValueDto> TopCouponsByUsage);
