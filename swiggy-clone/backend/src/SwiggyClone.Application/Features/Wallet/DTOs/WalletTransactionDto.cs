namespace SwiggyClone.Application.Features.Wallet.DTOs;

public sealed record WalletTransactionDto(
    Guid Id,
    int AmountPaise,
    short Type,
    short Source,
    Guid? ReferenceId,
    string Description,
    int BalanceAfterPaise,
    DateTimeOffset CreatedAt);
