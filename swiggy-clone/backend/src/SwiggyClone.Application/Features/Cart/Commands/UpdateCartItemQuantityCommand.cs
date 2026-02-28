using MediatR;
using SwiggyClone.Application.Features.Cart.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Cart.Commands;

public sealed record UpdateCartItemQuantityCommand(
    Guid UserId,
    string CartItemId,
    int Quantity) : IRequest<Result<CartDto>>;
