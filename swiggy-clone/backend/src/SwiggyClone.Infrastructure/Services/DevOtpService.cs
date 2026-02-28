using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SwiggyClone.Application.Common.Interfaces;

namespace SwiggyClone.Infrastructure.Services;

/// <summary>
/// Development-only OTP service that uses a fixed OTP (123456) for easy testing.
/// In production, replace with Twilio/MSG91 implementation.
/// </summary>
internal sealed class DevOtpService : IOtpService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<DevOtpService> _logger;

    private const string DevOtp = "123456";
    private const string CacheKeyPrefix = "otp:";
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    };

    public DevOtpService(IDistributedCache cache, ILogger<DevOtpService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<string> GenerateAndSendOtpAsync(string phoneNumber, CancellationToken ct = default)
    {
        var cacheKey = $"{CacheKeyPrefix}{phoneNumber}";
        await _cache.SetStringAsync(cacheKey, DevOtp, CacheOptions, ct);

        _logger.LogInformation(
            "[DEV] OTP {Otp} sent to {PhoneNumber}. This is a development-only service.",
            DevOtp, phoneNumber);

        return DevOtp;
    }

    public async Task<bool> VerifyOtpAsync(string phoneNumber, string otp, CancellationToken ct = default)
    {
        var cacheKey = $"{CacheKeyPrefix}{phoneNumber}";
        var storedOtp = await _cache.GetStringAsync(cacheKey, ct);

        if (storedOtp is null)
        {
            _logger.LogWarning("OTP verification failed for {PhoneNumber}: no OTP found in cache.", phoneNumber);
            return false;
        }

        var isValid = string.Equals(storedOtp, otp, StringComparison.Ordinal);

        if (isValid)
        {
            await _cache.RemoveAsync(cacheKey, ct);
            _logger.LogInformation("OTP verified successfully for {PhoneNumber}.", phoneNumber);
        }
        else
        {
            _logger.LogWarning("OTP verification failed for {PhoneNumber}: OTP mismatch.", phoneNumber);
        }

        return isValid;
    }
}
