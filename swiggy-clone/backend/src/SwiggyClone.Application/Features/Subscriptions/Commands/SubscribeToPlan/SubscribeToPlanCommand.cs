using MediatR;
using SwiggyClone.Application.Features.Subscriptions.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Subscriptions.Commands.SubscribeToPlan;

public sealed record SubscribeToPlanCommand(Guid UserId, Guid PlanId) : IRequest<Result<UserSubscriptionDto>>;
