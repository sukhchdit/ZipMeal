using MediatR;
using SwiggyClone.Application.Features.GroupOrders.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.GroupOrders.Commands;

public sealed record CreateGroupOrderCommand(
    Guid UserId,
    Guid RestaurantId,
    PaymentSplitType PaymentSplitType,
    Guid? DeliveryAddressId) : IRequest<Result<GroupOrderDto>>;
