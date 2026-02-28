using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Auth.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Auth.Commands;

internal sealed class LoginByPhoneCommandHandler(
    IAppDbContext db,
    IOtpService otpService,
    ITokenService tokenService)
    : IRequestHandler<LoginByPhoneCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(
        LoginByPhoneCommand request, CancellationToken ct)
    {
        var isValidOtp = await otpService.VerifyOtpAsync(request.PhoneNumber, request.Otp, ct);
        if (!isValidOtp)
            return Result<AuthResponse>.Failure("INVALID_OTP", "The OTP provided is invalid or expired.");

        var user = await db.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber, ct);
        if (user is null)
            return Result<AuthResponse>.Failure("USER_NOT_FOUND", "No account found with this phone number.");

        if (!user.IsActive)
            return Result<AuthResponse>.Failure("ACCOUNT_DISABLED", "This account has been deactivated.");

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
            user.AvatarUrl, user.Role.ToString(), user.IsVerified, user.LastLoginAt);

        return Result<AuthResponse>.Success(new AuthResponse(
            accessToken, rawRefreshToken, DateTime.UtcNow.AddMinutes(15), userDto));
    }
}
