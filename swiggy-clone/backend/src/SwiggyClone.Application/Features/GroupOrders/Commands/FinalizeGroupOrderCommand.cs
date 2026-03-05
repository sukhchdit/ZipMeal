using MediatR;
using SwiggyClone.Application.Features.Orders.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.GroupOrders.Commands;

public sealed record FinalizeGroupOrderCommand(
    Guid UserId,
    Guid GroupOrderId,
    Guid DeliveryAddressId,
    int PaymentMethod,
    string? CouponCode,
    string? SpecialInstructions) : IRequest<Result<OrderDto>>;
