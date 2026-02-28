using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Subscriptions.Commands.CancelSubscription;

public sealed record CancelSubscriptionCommand(Guid UserId) : IRequest<Result>;
