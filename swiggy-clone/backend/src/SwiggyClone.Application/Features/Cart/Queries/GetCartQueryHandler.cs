using MediatR;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Cart.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Cart.Queries;

internal sealed class GetCartQueryHandler(ICartService cartService)
    : IRequestHandler<GetCartQuery, Result<CartDto>>
{
    public async Task<Result<CartDto>> Handle(GetCartQuery request, CancellationToken ct)
    {
        return await cartService.GetCartAsync(request.UserId, ct);
    }
}
