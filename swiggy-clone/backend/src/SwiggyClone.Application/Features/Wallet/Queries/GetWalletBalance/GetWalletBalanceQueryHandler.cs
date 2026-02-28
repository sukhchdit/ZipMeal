using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Wallet.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Wallet.Queries.GetWalletBalance;

internal sealed class GetWalletBalanceQueryHandler(IAppDbContext db)
    : IRequestHandler<GetWalletBalanceQuery, Result<WalletDto>>
{
    public async Task<Result<WalletDto>> Handle(GetWalletBalanceQuery request, CancellationToken ct)
    {
        var wallet = await db.Wallets
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.UserId == request.UserId, ct);

        if (wallet is null)
        {
            // Return zero balance for users without a wallet (auto-create on first use)
            var dto = new WalletDto(Guid.Empty, request.UserId, 0, DateTimeOffset.UtcNow);
            return Result<WalletDto>.Success(dto);
        }

        return Result<WalletDto>.Success(
            new WalletDto(wallet.Id, wallet.UserId, wallet.BalancePaise, wallet.UpdatedAt));
    }
}
