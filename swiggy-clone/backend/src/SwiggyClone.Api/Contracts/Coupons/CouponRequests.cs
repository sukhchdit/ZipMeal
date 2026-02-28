namespace SwiggyClone.Api.Contracts.Coupons;

public sealed record CreateCouponRequest(
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
    short[] ApplicableOrderTypes,
    Guid[]? RestaurantIds);

public sealed record UpdateCouponRequest(
    string Title,
    string? Description,
    int? MaxDiscount,
    int MinOrderAmount,
    DateTimeOffset ValidFrom,
    DateTimeOffset ValidUntil,
    int? MaxUsages,
    int MaxUsagesPerUser,
    short[] ApplicableOrderTypes,
    Guid[]? RestaurantIds);

public sealed record ToggleCouponRequest(bool IsActive);
