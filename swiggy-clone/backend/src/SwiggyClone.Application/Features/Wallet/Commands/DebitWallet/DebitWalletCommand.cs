using MediatR;
using SwiggyClone.Application.Features.Wallet.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Wallet.Commands.DebitWallet;

public sealed record DebitWalletCommand(
    Guid UserId,
    int AmountPaise,
    Guid? ReferenceId,
    string Description) : IRequest<Result<WalletTransactionDto>>;
