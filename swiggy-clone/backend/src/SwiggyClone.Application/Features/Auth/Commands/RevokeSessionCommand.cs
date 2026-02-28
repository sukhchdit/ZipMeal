using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Auth.Commands;

public sealed record RevokeSessionCommand(Guid UserId, Guid SessionId) : IRequest<Result>;
