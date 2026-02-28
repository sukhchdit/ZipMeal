using MediatR;
using SwiggyClone.Application.Features.Cart.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Cart.Commands;

public sealed record RemoveCartItemCommand(Guid UserId, string CartItemId) : IRequest<Result<CartDto>>;
