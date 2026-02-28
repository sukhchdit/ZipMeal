using MediatR;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Cart.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Cart.Commands;

internal sealed class RemoveCartItemCommandHandler(ICartService cartService)
    : IRequestHandler<RemoveCartItemCommand, Result<CartDto>>
{
    public async Task<Result<CartDto>> Handle(RemoveCartItemCommand request, CancellationToken ct)
    {
        return await cartService.RemoveItemAsync(request.UserId, request.CartItemId, ct);
    }
}
