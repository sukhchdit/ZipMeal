using MediatR;
using SwiggyClone.Application.Features.Orders.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Orders.Commands;

public sealed record ReorderCommand(Guid UserId, Guid OrderId) : IRequest<Result<ReorderResultDto>>;
