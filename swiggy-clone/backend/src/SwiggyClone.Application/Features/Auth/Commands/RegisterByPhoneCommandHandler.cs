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

internal sealed class RegisterByPhoneCommandHandler(
    IAppDbContext db,
    IOtpService otpService,
    ITokenService tokenService,
    ISender sender,
    IEventBus eventBus,
    ILogger<RegisterByPhoneCommandHandler> logger)
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
            FullName = request.FullName,
            Role = UserRole.Customer,
            IsVerified = true,
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

        // Extremely unlikely fallback
        return ReferralCodeGenerator.Generate(Guid.NewGuid().ToString());
    }

    private async Task CreditReferralRewardsAsync(User referrer, User referee, CancellationToken ct)
    {
        try
        {
            // Credit referrer
            await sender.Send(new CreditWalletCommand(
                referrer.Id,
                ReferralConstants.RewardAmountPaise,
                (short)WalletTransactionSource.Referral,
                referee.Id,
                $"Referral reward: {referee.FullName} joined using your code"), ct);

            // Credit referee
            await sender.Send(new CreditWalletCommand(
                referee.Id,
                ReferralConstants.RewardAmountPaise,
                (short)WalletTransactionSource.Referral,
                referrer.Id,
                $"Welcome reward: joined via {referrer.FullName}'s referral"), ct);

            // Notify referrer via Kafka
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
