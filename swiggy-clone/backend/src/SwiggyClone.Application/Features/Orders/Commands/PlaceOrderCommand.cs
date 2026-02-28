using MediatR;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Orders.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Orders.Commands;

public sealed record PlaceOrderCommand(
    Guid UserId,
    Guid DeliveryAddressId,
    int PaymentMethod,
    string? SpecialInstructions,
    string? CouponCode,
    string? IdempotencyKey = null,
    DateTimeOffset? ScheduledDeliveryTime = null) : IRequest<Result<OrderDto>>, IIdempotent;
