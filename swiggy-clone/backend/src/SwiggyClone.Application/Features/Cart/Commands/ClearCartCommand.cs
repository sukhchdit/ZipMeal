using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Cart.Commands;

public sealed record ClearCartCommand(Guid UserId) : IRequest<Result>;
