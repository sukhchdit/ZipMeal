using MediatR;
using SwiggyClone.Application.Features.Subscriptions.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Subscriptions.Queries.CheckActiveSubscription;

public sealed record CheckActiveSubscriptionQuery(Guid UserId) : IRequest<Result<ActiveSubscriptionBenefitsDto>>;
