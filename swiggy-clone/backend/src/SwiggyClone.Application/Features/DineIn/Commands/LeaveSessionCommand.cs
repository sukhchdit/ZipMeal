using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.DineIn.Commands;

public sealed record LeaveSessionCommand(
    Guid UserId,
    Guid SessionId) : IRequest<Result>;
