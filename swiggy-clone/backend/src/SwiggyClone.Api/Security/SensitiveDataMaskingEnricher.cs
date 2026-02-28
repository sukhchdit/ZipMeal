using Serilog.Core;
using Serilog.Events;

namespace SwiggyClone.Api.Security;

/// <summary>
/// Serilog <see cref="ILogEventEnricher"/> that redacts log properties whose names
/// match known sensitive-data patterns (passwords, tokens, PII, etc.).
/// </summary>
public sealed class SensitiveDataMaskingEnricher : ILogEventEnricher
{
    private const string Redacted = "***REDACTED***";

    private static readonly HashSet<string> SensitivePropertyNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Password",
        "PasswordHash",
        "Token",
        "AccessToken",
        "RefreshToken",
        "Otp",
        "OtpCode",
        "Email",
        "PhoneNumber",
        "CardNumber",
        "Cvv",
        "SecretKey",
        "ApiKey",
        "Authorization",
    };

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var propertyKeys = logEvent.Properties.Keys.ToList();

        foreach (var key in propertyKeys)
        {
            if (SensitivePropertyNames.Contains(key))
            {
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(key, Redacted));
            }
        }
    }
}
