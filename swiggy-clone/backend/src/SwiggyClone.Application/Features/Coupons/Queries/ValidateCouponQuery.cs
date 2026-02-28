using MediatR;
using SwiggyClone.Application.Features.Coupons.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Coupons.Queries;

public sealed record ValidateCouponQuery(
    string Code,
    Guid UserId,
    int OrderSubtotal,
    OrderType OrderType,
    Guid RestaurantId) : IRequest<Result<CouponValidationDto>>;
