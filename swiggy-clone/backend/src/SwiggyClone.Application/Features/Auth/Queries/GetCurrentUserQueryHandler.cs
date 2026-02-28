using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Auth.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Auth.Queries;

internal sealed class GetCurrentUserQueryHandler(IAppDbContext db)
    : IRequestHandler<GetCurrentUserQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(
        GetCurrentUserQuery request, CancellationToken ct)
    {
        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.UserId, ct);

        if (user is null)
            return Result<UserDto>.Failure("USER_NOT_FOUND", "User not found.");

        return Result<UserDto>.Success(new UserDto(
            user.Id, user.PhoneNumber, user.Email, user.FullName,
            user.AvatarUrl, user.Role.ToString(), user.IsVerified, user.LastLoginAt));
    }
}
