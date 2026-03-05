using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Loyalty.Commands.EarnPoints;

public sealed record EarnPointsCommand(Guid UserId, Guid OrderId) : IRequest<Result<int>>;
