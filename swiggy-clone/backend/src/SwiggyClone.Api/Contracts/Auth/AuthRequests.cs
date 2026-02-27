namespace SwiggyClone.Api.Contracts.Auth;

public sealed record RegisterByPhoneRequest(string PhoneNumber, string Otp, string FullName);
public sealed record RegisterByEmailRequest(string Email, string Password, string FullName, string PhoneNumber);
public sealed record LoginByPhoneRequest(string PhoneNumber, string Otp);
public sealed record LoginByEmailRequest(string Email, string Password);
public sealed record SendOtpRequest(string PhoneNumber);
public sealed record RefreshTokenRequest(string RefreshToken);
public sealed record LogoutRequest(string RefreshToken);
public sealed record UpdateProfileRequest(string? FullName, string? Email, string? AvatarUrl);
public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);
