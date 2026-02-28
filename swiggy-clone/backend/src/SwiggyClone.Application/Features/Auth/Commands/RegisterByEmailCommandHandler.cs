using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Auth.DTOs;
using SwiggyClone.Application.Features.Wallet.Commands.CreditWallet;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;
using SwiggyClone.Shared.Constants;

namespace SwiggyClone.Application.Features.Auth.Commands;

internal sealed class RegisterByEmailCommandHandler(
    IAppDbContext db,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    ISender sender,
    IEventBus eventBus,
    ILogger<RegisterByEmailCommandHandler> logger)
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

        var referralCode = await GenerateUniqueReferralCodeAsync(request.FullName, ct);

        // Resolve referrer (silently ignore invalid codes)
        User? referrer = null;
        if (!string.IsNullOrWhiteSpace(request.ReferralCode))
        {
            referrer = await db.Users
                .FirstOrDefaultAsync(u => u.ReferralCode == request.ReferralCode, ct);
        }

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
            ReferralCode = referralCode,
            ReferredByUserId = referrer?.Id,
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

        // Credit referral rewards after user is persisted
        if (referrer is not null)
        {
            await CreditReferralRewardsAsync(referrer, user, ct);
        }

        var userDto = new UserDto(user.Id, user.PhoneNumber, user.Email, user.FullName,
            user.AvatarUrl, user.Role.ToString(), user.IsVerified, user.LastLoginAt, user.ReferralCode);

        return Result<AuthResponse>.Success(new AuthResponse(
            accessToken, rawRefreshToken, DateTime.UtcNow.AddMinutes(15), userDto));
    }

    private async Task<string> GenerateUniqueReferralCodeAsync(string fullName, CancellationToken ct)
    {
        for (var attempt = 0; attempt < 10; attempt++)
        {
            var code = ReferralCodeGenerator.Generate(fullName);
            var exists = await db.Users.AnyAsync(u => u.ReferralCode == code, ct);
            if (!exists) return code;
        }

        return ReferralCodeGenerator.Generate(Guid.NewGuid().ToString());
    }

    private async Task CreditReferralRewardsAsync(User referrer, User referee, CancellationToken ct)
    {
        try
        {
            await sender.Send(new CreditWalletCommand(
                referrer.Id,
                ReferralConstants.RewardAmountPaise,
                (short)WalletTransactionSource.Referral,
                referee.Id,
                $"Referral reward: {referee.FullName} joined using your code"), ct);

            await sender.Send(new CreditWalletCommand(
                referee.Id,
                ReferralConstants.RewardAmountPaise,
                (short)WalletTransactionSource.Referral,
                referrer.Id,
                $"Welcome reward: joined via {referrer.FullName}'s referral"), ct);

            await eventBus.PublishAsync(
                KafkaTopics.NotificationSend,
                referrer.Id.ToString(),
                new
                {
                    UserId = referrer.Id,
                    Title = "Referral Reward!",
                    Body = $"{referee.FullName} joined using your referral code. ₹100 credited to your wallet!",
                    Type = "referral_reward"
                }, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to credit referral rewards for referrer {ReferrerId} and referee {RefereeId}",
                referrer.Id, referee.Id);
        }
    }
}
