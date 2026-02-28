using AspNetCoreRateLimit;

namespace SwiggyClone.Api.Security;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddRateLimiting(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddMemoryCache();

        services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));

        services.AddInMemoryRateLimiting();

        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        return services;
    }
}
