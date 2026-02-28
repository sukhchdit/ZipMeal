using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;

namespace SwiggyClone.Application.Features.Wallet.Commands.GetOrCreateWallet;

internal sealed class GetOrCreateWalletCommandHandler(IAppDbContext db)
    : IRequestHandler<GetOrCreateWalletCommand, Domain.Entities.Wallet>
{
    public async Task<Domain.Entities.Wallet> Handle(GetOrCreateWalletCommand request, CancellationToken ct)
    {
        var wallet = await db.Wallets
            .FirstOrDefaultAsync(w => w.UserId == request.UserId, ct);

        if (wallet is not null)
        {
            return wallet;
        }

        wallet = Domain.Entities.Wallet.Create(request.UserId);
        db.Wallets.Add(wallet);
        await db.SaveChangesAsync(ct);

        return wallet;
    }
}
