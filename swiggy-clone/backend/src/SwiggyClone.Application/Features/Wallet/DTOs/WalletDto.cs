namespace SwiggyClone.Application.Features.Wallet.DTOs;

public sealed record WalletDto(Guid Id, Guid UserId, int BalancePaise, DateTimeOffset UpdatedAt);
