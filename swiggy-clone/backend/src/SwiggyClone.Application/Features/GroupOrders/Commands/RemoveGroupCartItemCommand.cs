using MediatR;
using SwiggyClone.Application.Features.Cart.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.GroupOrders.Commands;

public sealed record RemoveGroupCartItemCommand(
    Guid UserId,
    Guid GroupOrderId,
    string CartItemId) : IRequest<Result<CartDto>>;
