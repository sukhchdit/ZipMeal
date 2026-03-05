using MediatR;
using SwiggyClone.Application.Features.Loyalty.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Loyalty.Queries.GetTransactions;

public sealed record GetLoyaltyTransactionsQuery(
    Guid UserId,
    int Page,
    int PageSize,
    short? Type) : IRequest<Result<PagedResult<LoyaltyTransactionDto>>>;
