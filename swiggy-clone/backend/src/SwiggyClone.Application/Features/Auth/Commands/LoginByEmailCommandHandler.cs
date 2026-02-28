using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Auth.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Auth.Commands;

internal sealed class LoginByEmailCommandHandler(
    IAppDbContext db,
    IPasswordHasher passwordHasher,
    ITokenService tokenService)
    : IRequestHandler<LoginByEmailCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(
        LoginByEmailCommand request, CancellationToken ct)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, ct);

        if (user is null)
            return Result<AuthResponse>.Failure("INVALID_CREDENTIALS", "Invalid email or password.");

        if (!user.IsActive)
            return Result<AuthResponse>.Failure("ACCOUNT_DISABLED", "This account has been deactivated.");

        if (user.PasswordHash is null)
            return Result<AuthResponse>.Failure("NO_PASSWORD", "This account uses phone OTP login. Please log in with your phone number.");

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result<AuthResponse>.Failure("INVALID_CREDENTIALS", "Invalid email or password.");

        user.LastLoginAt = DateTimeOffset.UtcNow;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        var accessToken = tokenService.GenerateAccessToken(user.Id, user.Role.ToString(), user.Email, user.PhoneNumber);
        var rawRefreshToken = tokenService.GenerateRefreshToken();
        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            TokenHash = tokenService.HashToken(rawRefreshToken),
            DeviceInfo = request.DeviceInfo,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(7),
            CreatedAt = DateTimeOffset.UtcNow
        };

        db.RefreshTokens.Add(refreshTokenEntity);
        await db.SaveChangesAsync(ct);

        var userDto = new UserDto(user.Id, user.PhoneNumber, user.Email, user.FullName,
            user.AvatarUrl, user.Role.ToString(), user.IsVerified, user.LastLoginAt, user.ReferralCode);

        return Result<AuthResponse>.Success(new AuthResponse(
            accessToken, rawRefreshToken, DateTime.UtcNow.AddMinutes(15), userDto));
    }
}
