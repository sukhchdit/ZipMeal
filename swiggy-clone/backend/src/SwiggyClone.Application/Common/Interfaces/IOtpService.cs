namespace SwiggyClone.Application.Common.Interfaces;

public interface IOtpService
{
    Task<string> GenerateAndSendOtpAsync(string phoneNumber, CancellationToken ct = default);
    Task<bool> VerifyOtpAsync(string phoneNumber, string otp, CancellationToken ct = default);
}
