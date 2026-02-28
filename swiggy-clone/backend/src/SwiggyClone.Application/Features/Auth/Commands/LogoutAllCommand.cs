using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Auth.Commands;

public sealed record LogoutAllCommand(Guid UserId) : IRequest<Result>;
