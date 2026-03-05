using MediatR;
using SwiggyClone.Application.Features.Loyalty.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Loyalty.Commands.AdjustPoints;

public sealed record AdjustPointsCommand(
    Guid UserId,
    int Points,
    string Description) : IRequest<Result<LoyaltyTransactionDto>>;
