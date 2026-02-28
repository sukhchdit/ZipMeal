using MediatR;
using SwiggyClone.Application.Features.Wallet.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Wallet.Queries.GetWalletBalance;

public sealed record GetWalletBalanceQuery(Guid UserId) : IRequest<Result<WalletDto>>;
