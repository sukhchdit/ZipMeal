using MediatR;
using SwiggyClone.Application.Features.Admin.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Admin.Queries;

public sealed record GetAdminUserDetailQuery(Guid UserId) : IRequest<Result<AdminUserDto>>;
