using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Auth.Commands;

public sealed record LogoutCommand(string RefreshToken) : IRequest<Result>;
