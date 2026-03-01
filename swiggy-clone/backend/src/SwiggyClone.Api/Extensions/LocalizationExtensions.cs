using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using SwiggyClone.Api.Filters;

namespace SwiggyClone.Api.Extensions;

/// <summary>
/// Extension methods for configuring request/response localization
/// using .resx resource files and the Accept-Language header.
/// </summary>
public static class LocalizationExtensions
{
    private static readonly string[] SupportedCultures = ["en", "hi"];

    /// <summary>
    /// Registers localization services, request culture providers, and the
    /// <see cref="ResponseLocalizationFilter"/> as a global MVC filter.
    /// </summary>
    public static IServiceCollection AddAppLocalization(this IServiceCollection services)
    {
        services.AddLocalization(options => options.ResourcesPath = "Resources");

        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.SetDefaultCulture("en")
                .AddSupportedCultures(SupportedCultures)
                .AddSupportedUICultures(SupportedCultures);
            options.ApplyCurrentCultureToResponseHeaders = true;
        });

        // Register the response localization filter globally
        services.AddScoped<ResponseLocalizationFilter>();
        services.Configure<Microsoft.AspNetCore.Mvc.MvcOptions>(options =>
        {
            options.Filters.AddService<ResponseLocalizationFilter>();
        });

        return services;
    }

    /// <summary>
    /// Adds the ASP.NET Core request localization middleware, reading the
    /// <c>Accept-Language</c> header to determine the request culture.
    /// </summary>
    public static IApplicationBuilder UseAppLocalization(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices
            .GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
        app.UseRequestLocalization(options);
        return app;
    }
}
