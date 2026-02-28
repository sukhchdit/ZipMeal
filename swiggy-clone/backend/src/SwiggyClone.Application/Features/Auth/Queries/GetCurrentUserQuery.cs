using MediatR;
using SwiggyClone.Application.Features.Auth.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Auth.Queries;

public sealed record GetCurrentUserQuery(Guid UserId) : IRequest<Result<UserDto>>;
