using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Admin.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Admin.Queries;

internal sealed class GetAdminUserDetailQueryHandler(IAppDbContext db)
    : IRequestHandler<GetAdminUserDetailQuery, Result<AdminUserDto>>
{
    public async Task<Result<AdminUserDto>> Handle(
        GetAdminUserDetailQuery request, CancellationToken ct)
    {
        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.UserId, ct);

        if (user is null)
            return Result<AdminUserDto>.Failure("USER_NOT_FOUND", "User not found.");

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
