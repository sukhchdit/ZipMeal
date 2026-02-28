using MediatR;
using SwiggyClone.Application.Features.Subscriptions.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Subscriptions.Queries.GetMySubscription;

public sealed record GetMySubscriptionQuery(Guid UserId) : IRequest<Result<UserSubscriptionDto?>>;
