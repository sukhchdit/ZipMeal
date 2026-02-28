using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Auth.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Auth.Commands;

internal sealed class UpdateProfileCommandHandler(IAppDbContext db)
    : IRequestHandler<UpdateProfileCommand, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(
        UpdateProfileCommand request, CancellationToken ct)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, ct);
        if (user is null)
            return Result<UserDto>.Failure("USER_NOT_FOUND", "User not found.");

        if (request.FullName is not null)
            user.FullName = request.FullName;

        if (request.Email is not null)
        {
            var emailTaken = await db.Users
                .AnyAsync(u => u.Email == request.Email && u.Id != request.UserId, ct);
            if (emailTaken)
                return Result<UserDto>.Failure("EMAIL_TAKEN", "This email is already in use.");
            user.Email = request.Email;
        }

        if (request.AvatarUrl is not null)
            user.AvatarUrl = request.AvatarUrl;

        user.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct);

        return Result<UserDto>.Success(new UserDto(
            user.Id, user.PhoneNumber, user.Email, user.FullName,
            user.AvatarUrl, user.Role.ToString(), user.IsVerified, user.LastLoginAt, user.ReferralCode));
    }
}
