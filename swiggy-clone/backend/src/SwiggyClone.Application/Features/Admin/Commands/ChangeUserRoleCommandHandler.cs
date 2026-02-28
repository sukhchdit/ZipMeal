using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Admin.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Admin.Commands;

internal sealed class ChangeUserRoleCommandHandler(IAppDbContext db)
    : IRequestHandler<ChangeUserRoleCommand, Result<AdminUserDto>>
{
    public async Task<Result<AdminUserDto>> Handle(ChangeUserRoleCommand request, CancellationToken ct)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Id == request.TargetUserId, ct);

        if (user is null)
            return Result<AdminUserDto>.Failure("USER_NOT_FOUND", "User not found.");

        user.Role = request.NewRole;
        await db.SaveChangesAsync(ct);

        return Result<AdminUserDto>.Success(new AdminUserDto(
            user.Id,
            user.FullName,
            user.PhoneNumber,
            user.Email,
            user.AvatarUrl,
            user.Role,
            user.IsVerified,
            user.IsActive,
            user.LastLoginAt,
            user.CreatedAt));
    }
}
