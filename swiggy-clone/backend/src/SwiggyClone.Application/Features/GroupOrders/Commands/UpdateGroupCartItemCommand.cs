using MediatR;
using SwiggyClone.Application.Features.Cart.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.GroupOrders.Commands;

public sealed record UpdateGroupCartItemCommand(
    Guid UserId,
    Guid GroupOrderId,
    string CartItemId,
    int Quantity) : IRequest<Result<CartDto>>;
