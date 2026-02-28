using MediatR;
using SwiggyClone.Application.Features.Coupons.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Coupons.Commands;

public sealed record UpdateCouponCommand(
    Guid Id,
    string Title,
    string? Description,
    int? MaxDiscount,
    int MinOrderAmount,
    DateTimeOffset ValidFrom,
    DateTimeOffset ValidUntil,
    int? MaxUsages,
    int MaxUsagesPerUser,
    short[] ApplicableOrderTypes,
    Guid[]? RestaurantIds) : IRequest<Result<AdminCouponDto>>;
