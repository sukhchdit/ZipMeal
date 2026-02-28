using MediatR;
using SwiggyClone.Application.Features.Wallet.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Wallet.Queries.GetWalletTransactions;

public sealed record GetWalletTransactionsQuery(
    Guid UserId,
    int Page,
    int PageSize) : IRequest<Result<PagedResult<WalletTransactionDto>>>;
