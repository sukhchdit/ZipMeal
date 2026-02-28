using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Subscriptions.Commands.TogglePlan;

public sealed record TogglePlanCommand(Guid Id, bool IsActive) : IRequest<Result>;
