using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Auth.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Auth.Commands;

internal sealed class RegisterByPhoneCommandHandler(
    IAppDbContext db,
    IOtpService otpService,
    ITokenService tokenService)
    : IRequestHandler<RegisterByPhoneCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(
        RegisterByPhoneCommand request, CancellationToken ct)
    {
        var isValidOtp = await otpService.VerifyOtpAsync(request.PhoneNumber, request.Otp, ct);
        if (!isValidOtp)
            return Result<AuthResponse>.Failure("INVALID_OTP", "The OTP provided is invalid or expired.");

        var existingUser = await db.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber, ct);
        if (existingUser is not null)
            return Result<AuthResponse>.Failure("PHONE_TAKEN", "An account with this phone number already exists.");

        var user = new User
        {
            Id = Guid.CreateVersion7(),
            PhoneNumber = request.PhoneNumber,
            FullName = request.FullName,
            Role = UserRole.Customer,
            IsVerified = true,
            IsActive = true,
            LastLoginAt = DateTimeOffset.UtcNow,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        db.Users.Add(user);

        var accessToken = tokenService.GenerateAccessToken(user.Id, user.Role.ToString(), user.Email, user.PhoneNumber);
        var rawRefreshToken = tokenService.GenerateRefreshToken();
        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            TokenHash = tokenService.HashToken(rawRefreshToken),
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
