using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Auth.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Auth.Commands;

internal sealed class RegisterByEmailCommandHandler(
    IAppDbContext db,
    IPasswordHasher passwordHasher,
    ITokenService tokenService)
    : IRequestHandler<RegisterByEmailCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(
        RegisterByEmailCommand request, CancellationToken ct)
    {
        var existingUser = await db.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, ct);
        if (existingUser is not null)
            return Result<AuthResponse>.Failure("EMAIL_TAKEN", "An account with this email already exists.");

        var existingPhone = await db.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber, ct);
        if (existingPhone is not null)
            return Result<AuthResponse>.Failure("PHONE_TAKEN", "An account with this phone number already exists.");

        var user = new User
        {
            Id = Guid.CreateVersion7(),
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            FullName = request.FullName,
            PasswordHash = passwordHasher.Hash(request.Password),
            Role = UserRole.Customer,
            IsVerified = false,
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
