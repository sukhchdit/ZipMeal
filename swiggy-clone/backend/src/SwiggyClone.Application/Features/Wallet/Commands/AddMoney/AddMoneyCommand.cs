using MediatR;
using SwiggyClone.Application.Features.Wallet.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Wallet.Commands.AddMoney;

public sealed record AddMoneyCommand(Guid UserId, int AmountPaise) : IRequest<Result<WalletTransactionDto>>;
