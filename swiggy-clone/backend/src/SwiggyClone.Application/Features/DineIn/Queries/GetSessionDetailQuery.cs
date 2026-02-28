using MediatR;
using SwiggyClone.Application.Features.DineIn.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.DineIn.Queries;

public sealed record GetSessionDetailQuery(
    Guid UserId,
    Guid SessionId) : IRequest<Result<DineInSessionDto>>;
