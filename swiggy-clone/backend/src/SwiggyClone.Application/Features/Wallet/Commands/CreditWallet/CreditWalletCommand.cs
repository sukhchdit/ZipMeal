using MediatR;
using SwiggyClone.Application.Features.Wallet.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Wallet.Commands.CreditWallet;

public sealed record CreditWalletCommand(
    Guid UserId,
    int AmountPaise,
    short Source,
    Guid? ReferenceId,
    string Description) : IRequest<Result<WalletTransactionDto>>;
