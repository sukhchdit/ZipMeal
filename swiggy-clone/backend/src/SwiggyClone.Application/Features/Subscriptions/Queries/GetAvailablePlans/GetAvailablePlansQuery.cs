using MediatR;
using SwiggyClone.Application.Features.Subscriptions.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Subscriptions.Queries.GetAvailablePlans;

public sealed record GetAvailablePlansQuery() : IRequest<Result<List<SubscriptionPlanDto>>>;
