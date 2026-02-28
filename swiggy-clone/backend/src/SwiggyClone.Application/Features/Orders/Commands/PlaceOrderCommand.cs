using MediatR;
using SwiggyClone.Application.Features.Orders.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Orders.Commands;

public sealed record PlaceOrderCommand(
    Guid UserId,
    Guid DeliveryAddressId,
    int PaymentMethod,
    string? SpecialInstructions,
    string? CouponCode) : IRequest<Result<OrderDto>>;
