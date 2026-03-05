using MediatR;

namespace SwiggyClone.Application.Features.Loyalty.Commands.GetOrCreateLoyaltyAccount;

internal sealed record GetOrCreateLoyaltyAccountCommand(Guid UserId) : IRequest<Domain.Entities.LoyaltyAccount>;
