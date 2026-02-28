using MediatR;
using SwiggyClone.Application.Features.DineIn.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.DineIn.Commands;

public sealed record JoinSessionCommand(
    Guid UserId,
    string SessionCode) : IRequest<Result<DineInSessionDto>>;
