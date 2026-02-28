using MediatR;
using SwiggyClone.Application.Features.Coupons.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Coupons.Commands;

public sealed record CreateCouponCommand(
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
    Guid[]? RestaurantIds) : IRequest<Result<AdminCouponDto>>;
