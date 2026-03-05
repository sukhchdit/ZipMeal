using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.GroupOrders.Commands;

public sealed record SetParticipantReadyCommand(
    Guid UserId,
    Guid GroupOrderId) : IRequest<Result>;
