using MediatR;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Wallet.Commands.GetOrCreateWallet;
using SwiggyClone.Application.Features.Wallet.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Wallet.Commands.AddMoney;

internal sealed class AddMoneyCommandHandler(IAppDbContext db, ISender sender)
    : IRequestHandler<AddMoneyCommand, Result<WalletTransactionDto>>
{
    public async Task<Result<WalletTransactionDto>> Handle(AddMoneyCommand request, CancellationToken ct)
    {
        var wallet = await sender.Send(new GetOrCreateWalletCommand(request.UserId), ct);

        wallet.Credit(request.AmountPaise);

        var txn = WalletTransaction.Create(
            wallet.Id,
            request.AmountPaise,
            WalletTransactionType.Credit,
            WalletTransactionSource.AddMoney,
            referenceId: null,
            "Added money to wallet",
            wallet.BalancePaise);

        db.WalletTransactions.Add(txn);
        await db.SaveChangesAsync(ct);

        var dto = new WalletTransactionDto(
            txn.Id, txn.AmountPaise, (short)txn.Type, (short)txn.Source,
            txn.ReferenceId, txn.Description, txn.BalanceAfterPaise, txn.CreatedAt);

        return Result<WalletTransactionDto>.Success(dto);
    }
}
