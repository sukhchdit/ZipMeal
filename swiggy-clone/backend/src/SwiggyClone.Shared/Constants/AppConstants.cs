namespace SwiggyClone.Shared.Constants;

public static class AppConstants
{
    public const string ApiVersion = "v1";
    public const string ApiRoutePrefix = $"api/{ApiVersion}";
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;
    public const string CorrelationIdHeader = "X-Correlation-Id";
    public const string DefaultCurrency = "INR";
    public const int OtpLength = 6;
    public const int OtpExpiryMinutes = 5;
    public const int MaxOtpAttemptsPerHour = 5;
    public const int MaxLoginAttemptsPerWindow = 10;
    public const int LoginAttemptWindowMinutes = 15;
}
