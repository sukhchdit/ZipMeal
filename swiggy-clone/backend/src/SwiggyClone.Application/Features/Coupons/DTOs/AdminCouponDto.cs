namespace SwiggyClone.Application.Features.Coupons.DTOs;

public sealed record AdminCouponDto(
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
    int? MaxUsages,
    int MaxUsagesPerUser,
    int CurrentUsages,
    short[] ApplicableOrderTypes,
    Guid[]? RestaurantIds,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
