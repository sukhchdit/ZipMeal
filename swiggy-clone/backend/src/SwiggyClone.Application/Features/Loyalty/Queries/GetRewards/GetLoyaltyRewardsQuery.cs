using MediatR;
using SwiggyClone.Application.Features.Loyalty.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Loyalty.Queries.GetRewards;

public sealed record GetLoyaltyRewardsQuery : IRequest<Result<IReadOnlyList<LoyaltyRewardDto>>>;
