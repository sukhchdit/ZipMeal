using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Wallet.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Wallet.Commands.DebitWallet;

internal sealed class DebitWalletCommandHandler(IAppDbContext db)
    : IRequestHandler<DebitWalletCommand, Result<WalletTransactionDto>>
{
    public async Task<Result<WalletTransactionDto>> Handle(DebitWalletCommand request, CancellationToken ct)
    {
        var wallet = await db.Wallets
            .FirstOrDefaultAsync(w => w.UserId == request.UserId, ct);

        if (wallet is null)
        {
            return Result<WalletTransactionDto>.Failure("WALLET_NOT_FOUND", "Wallet not found for user.");
        }

        if (wallet.BalancePaise < request.AmountPaise)
        {
            return Result<WalletTransactionDto>.Failure("INSUFFICIENT_BALANCE",
                $"Insufficient wallet balance. Available: {wallet.BalancePaise}, Requested: {request.AmountPaise}");
        }

        wallet.Debit(request.AmountPaise);

        var txn = WalletTransaction.Create(
            wallet.Id,
            request.AmountPaise,
            WalletTransactionType.Debit,
            WalletTransactionSource.OrderPayment,
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
