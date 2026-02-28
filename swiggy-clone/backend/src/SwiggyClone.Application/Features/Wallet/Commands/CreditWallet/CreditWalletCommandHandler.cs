using MediatR;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Wallet.Commands.GetOrCreateWallet;
using SwiggyClone.Application.Features.Wallet.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Wallet.Commands.CreditWallet;

internal sealed class CreditWalletCommandHandler(IAppDbContext db, ISender sender)
    : IRequestHandler<CreditWalletCommand, Result<WalletTransactionDto>>
{
    public async Task<Result<WalletTransactionDto>> Handle(CreditWalletCommand request, CancellationToken ct)
    {
        var wallet = await sender.Send(new GetOrCreateWalletCommand(request.UserId), ct);

        wallet.Credit(request.AmountPaise);

        var txn = WalletTransaction.Create(
            wallet.Id,
            request.AmountPaise,
            WalletTransactionType.Credit,
            (WalletTransactionSource)request.Source,
            request.ReferenceId,
            request.Description,
            wallet.BalancePaise);

        db.WalletTransactions.Add(txn);
        await db.SaveChangesAsync(ct);

        var dto = new WalletTransactionDto(
            txn.Id, txn.AmountPaise, (short)txn.Type, (short)txn.Source,
            txn.ReferenceId, txn.Description, txn.BalanceAfterPaise, txn.CreatedAt);

        return Result<WalletTransactionDto>.Success(dto);
    }
}
