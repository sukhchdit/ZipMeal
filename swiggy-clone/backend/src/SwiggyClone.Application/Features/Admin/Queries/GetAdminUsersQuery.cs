using MediatR;
using SwiggyClone.Application.Features.Admin.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Admin.Queries;

public sealed record GetAdminUsersQuery(
    string? Search,
    UserRole? RoleFilter,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PagedResult<AdminUserDto>>>;
