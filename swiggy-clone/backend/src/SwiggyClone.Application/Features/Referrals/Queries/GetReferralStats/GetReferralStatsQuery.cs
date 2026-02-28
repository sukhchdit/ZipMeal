using MediatR;
using SwiggyClone.Application.Features.Referrals.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Referrals.Queries.GetReferralStats;

public sealed record GetReferralStatsQuery(Guid UserId) : IRequest<Result<ReferralStatsDto>>;
