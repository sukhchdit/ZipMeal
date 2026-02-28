using MediatR;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Cart.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Cart.Commands;

internal sealed class UpdateCartItemQuantityCommandHandler(ICartService cartService)
    : IRequestHandler<UpdateCartItemQuantityCommand, Result<CartDto>>
{
    public async Task<Result<CartDto>> Handle(UpdateCartItemQuantityCommand request, CancellationToken ct)
    {
        return await cartService.UpdateQuantityAsync(request.UserId, request.CartItemId, request.Quantity, ct);
    }
}
