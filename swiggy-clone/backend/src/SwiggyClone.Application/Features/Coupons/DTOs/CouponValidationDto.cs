namespace SwiggyClone.Application.Features.Coupons.DTOs;

public sealed record CouponValidationDto(
    Guid Id,
    string Code,
    string Title,
    string? Description,
    int DiscountAmount,
    bool IsValid,
    string? InvalidReason);
