using MediatR;
using SwiggyClone.Application.Features.Auth.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Auth.Queries;

public sealed record GetActiveSessionsQuery(
    Guid UserId,
    string? CurrentTokenHash = null) : IRequest<Result<List<SessionDto>>>;
