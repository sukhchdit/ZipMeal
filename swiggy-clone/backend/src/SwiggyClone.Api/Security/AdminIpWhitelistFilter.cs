using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SwiggyClone.Api.Security;

/// <summary>
/// Authorization filter that restricts admin endpoints to a configurable set of IP addresses.
/// When <c>Security:AdminIpWhitelist</c> is empty, the check is skipped (development mode).
/// Returns a 403 Problem Details JSON response on mismatch.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AdminIpWhitelistAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var whitelist = configuration.GetSection("Security:AdminIpWhitelist").Get<string[]>() ?? [];

        if (whitelist.Length == 0)
        {
            return; // No whitelist configured — skip check (development)
        }

        var remoteIp = GetClientIpAddress(context.HttpContext);

        if (remoteIp is null || !whitelist.Contains(remoteIp, StringComparer.Ordinal))
        {
            context.Result = new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Title = "Forbidden",
                Detail = "Your IP address is not authorized to access admin endpoints.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
            })
            {
                StatusCode = StatusCodes.Status403Forbidden,
            };
        }
    }

    private static string? GetClientIpAddress(HttpContext context)
    {
        // Prefer X-Real-IP set by nginx, fall back to RemoteIpAddress
        if (context.Request.Headers.TryGetValue("X-Real-IP", out var realIp)
            && !string.IsNullOrWhiteSpace(realIp))
        {
            return realIp.ToString();
        }

        return context.Connection.RemoteIpAddress?.MapToIPv4().ToString();
    }
}
