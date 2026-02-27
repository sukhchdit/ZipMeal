using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Auth.Commands;

public sealed record DeleteAccountCommand(Guid UserId) : IRequest<Result>;
