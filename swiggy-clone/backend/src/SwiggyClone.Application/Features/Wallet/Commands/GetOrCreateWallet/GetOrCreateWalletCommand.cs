using MediatR;

namespace SwiggyClone.Application.Features.Wallet.Commands.GetOrCreateWallet;

/// <summary>
/// Internal command to retrieve or create a wallet for a user.
/// </summary>
internal sealed record GetOrCreateWalletCommand(Guid UserId) : IRequest<Domain.Entities.Wallet>;
