using MediatR;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Cart.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Cart.Commands;

internal sealed class AddToCartCommandHandler(ICartService cartService)
    : IRequestHandler<AddToCartCommand, Result<CartDto>>
{
    public async Task<Result<CartDto>> Handle(AddToCartCommand request, CancellationToken ct)
    {
        var item = new AddToCartItem(
            request.RestaurantId,
            request.MenuItemId,
            request.VariantId,
            request.Quantity,
            request.SpecialInstructions,
            request.Addons);

        return await cartService.AddToCartAsync(request.UserId, item, ct);
    }
}
