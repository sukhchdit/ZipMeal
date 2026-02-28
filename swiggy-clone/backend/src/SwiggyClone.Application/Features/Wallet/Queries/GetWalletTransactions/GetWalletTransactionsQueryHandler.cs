using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Wallet.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Wallet.Queries.GetWalletTransactions;

internal sealed class GetWalletTransactionsQueryHandler(IAppDbContext db)
    : IRequestHandler<GetWalletTransactionsQuery, Result<PagedResult<WalletTransactionDto>>>
{
    public async Task<Result<PagedResult<WalletTransactionDto>>> Handle(
        GetWalletTransactionsQuery request, CancellationToken ct)
    {
        var wallet = await db.Wallets
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.UserId == request.UserId, ct);

        if (wallet is null)
        {
            // No wallet yet — return empty page
            var empty = new PagedResult<WalletTransactionDto>([], 0, request.Page, request.PageSize);
            return Result<PagedResult<WalletTransactionDto>>.Success(empty);
        }

        var query = db.WalletTransactions
            .AsNoTracking()
            .Where(t => t.WalletId == wallet.Id)
            .OrderByDescending(t => t.CreatedAt);

        var totalCount = await query.CountAsync(ct);

        var transactions = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new WalletTransactionDto(
                t.Id, t.AmountPaise, (short)t.Type, (short)t.Source,
                t.ReferenceId, t.Description, t.BalanceAfterPaise, t.CreatedAt))
            .ToListAsync(ct);

        var result = new PagedResult<WalletTransactionDto>(transactions, totalCount, request.Page, request.PageSize);
        return Result<PagedResult<WalletTransactionDto>>.Success(result);
    }
}
