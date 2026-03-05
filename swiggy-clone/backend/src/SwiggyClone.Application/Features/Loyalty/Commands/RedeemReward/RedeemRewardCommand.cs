using MediatR;
using SwiggyClone.Application.Features.Loyalty.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Loyalty.Commands.RedeemReward;

public sealed record RedeemRewardCommand(Guid UserId, Guid RewardId) : IRequest<Result<LoyaltyTransactionDto>>;
