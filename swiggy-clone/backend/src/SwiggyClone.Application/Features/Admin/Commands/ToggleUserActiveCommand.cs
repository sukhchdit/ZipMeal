using MediatR;
using SwiggyClone.Application.Features.Admin.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Admin.Commands;

public sealed record ToggleUserActiveCommand(
    Guid TargetUserId,
    bool IsActive) : IRequest<Result<AdminUserDto>>;
