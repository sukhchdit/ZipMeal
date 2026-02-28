using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Auth.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Auth.Commands;

internal sealed class RefreshTokenCommandHandler(
    IAppDbContext db,
    ITokenService tokenService)
    : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(
        RefreshTokenCommand request, CancellationToken ct)
    {
        var tokenHash = tokenService.HashToken(request.RefreshToken);

        var existingToken = await db.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, ct);

        if (existingToken is null)
            return Result<AuthResponse>.Failure("INVALID_TOKEN", "The refresh token is invalid.");

        // Reuse detection: if token is already revoked, revoke ALL tokens for this user
        if (existingToken.RevokedAt is not null)
        {
            var allUserTokens = await db.RefreshTokens
                .Where(rt => rt.UserId == existingToken.UserId && rt.RevokedAt == null)
                .ToListAsync(ct);

            foreach (var t in allUserTokens)
                t.RevokedAt = DateTimeOffset.UtcNow;

            await db.SaveChangesAsync(ct);
            return Result<AuthResponse>.Failure("TOKEN_REUSE_DETECTED", "Potential token theft detected. All sessions have been revoked.");
        }

        if (existingToken.ExpiresAt <= DateTimeOffset.UtcNow)
            return Result<AuthResponse>.Failure("TOKEN_EXPIRED", "The refresh token has expired.");

        var user = existingToken.User;
        if (!user.IsActive)
            return Result<AuthResponse>.Failure("ACCOUNT_DISABLED", "This account has been deactivated.");

        // Rotate: revoke old, issue new
        existingToken.RevokedAt = DateTimeOffset.UtcNow;

        var newAccessToken = tokenService.GenerateAccessToken(user.Id, user.Role.ToString(), user.Email, user.PhoneNumber);
        var newRawRefreshToken = tokenService.GenerateRefreshToken();
        var newRefreshTokenEntity = new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            TokenHash = tokenService.HashToken(newRawRefreshToken),
            DeviceInfo = existingToken.DeviceInfo,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(7),
            CreatedAt = DateTimeOffset.UtcNow
        };

        db.RefreshTokens.Add(newRefreshTokenEntity);
        await db.SaveChangesAsync(ct);

        var userDto = new UserDto(user.Id, user.PhoneNumber, user.Email, user.FullName,
            user.AvatarUrl, user.Role.ToString(), user.IsVerified, user.LastLoginAt);

        return Result<AuthResponse>.Success(new AuthResponse(
            newAccessToken, newRawRefreshToken, DateTime.UtcNow.AddMinutes(15), userDto));
    }
}
