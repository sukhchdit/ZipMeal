using MediatR;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Subscriptions.DTOs;
using SwiggyClone.Shared;
using SwiggyClone.Shared.Constants;

namespace SwiggyClone.Application.Features.Subscriptions.Queries.GetAvailablePlans;

public sealed record GetAvailablePlansQuery() : IRequest<Result<List<SubscriptionPlanDto>>>, ICacheable
{
    public string CacheKey => CacheKeys.AvailablePlansKey;
    public int CacheExpirationMinutes => 30;
}
