using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Loyalty.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Loyalty.Queries.GetTransactions;

internal sealed class GetLoyaltyTransactionsQueryHandler(IAppDbContext db)
    : IRequestHandler<GetLoyaltyTransactionsQuery, Result<PagedResult<LoyaltyTransactionDto>>>
{
    public async Task<Result<PagedResult<LoyaltyTransactionDto>>> Handle(
        GetLoyaltyTransactionsQuery request, CancellationToken ct)
    {
        var account = await db.LoyaltyAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.UserId == request.UserId, ct);

        if (account is null)
        {
            var empty = new PagedResult<LoyaltyTransactionDto>([], 0, request.Page, request.PageSize);
            return Result<PagedResult<LoyaltyTransactionDto>>.Success(empty);
        }

        var query = db.LoyaltyTransactions
            .AsNoTracking()
            .Where(t => t.LoyaltyAccountId == account.Id);

        if (request.Type.HasValue)
        {
            var typeFilter = (LoyaltyTransactionType)request.Type.Value;
            query = query.Where(t => t.Type == typeFilter);
        }

        query = query.OrderByDescending(t => t.CreatedAt);

        var totalCount = await query.CountAsync(ct);

        var transactions = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new LoyaltyTransactionDto(
                t.Id, t.Points, (short)t.Type, (short)t.Source,
                t.ReferenceId, t.Description, t.BalanceAfter, t.CreatedAt))
            .ToListAsync(ct);

        var result = new PagedResult<LoyaltyTransactionDto>(
            transactions, totalCount, request.Page, request.PageSize);

        return Result<PagedResult<LoyaltyTransactionDto>>.Success(result);
    }
}
