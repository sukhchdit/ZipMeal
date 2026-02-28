using MediatR;
using SwiggyClone.Application.Features.Admin.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Admin.Commands;

public sealed record ChangeUserRoleCommand(
    Guid TargetUserId,
    UserRole NewRole) : IRequest<Result<AdminUserDto>>;
