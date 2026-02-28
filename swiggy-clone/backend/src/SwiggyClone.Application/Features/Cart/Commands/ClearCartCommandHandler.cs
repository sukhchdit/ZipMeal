using MediatR;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Cart.Commands;

internal sealed class ClearCartCommandHandler(ICartService cartService)
    : IRequestHandler<ClearCartCommand, Result>
{
    public async Task<Result> Handle(ClearCartCommand request, CancellationToken ct)
    {
        return await cartService.ClearCartAsync(request.UserId, ct);
    }
}
