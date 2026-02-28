using System.Security.Claims;
using System.Text.RegularExpressions;

namespace SwiggyClone.Api.Middleware;

/// <summary>
/// Logs a structured audit trail for mutating HTTP methods (POST, PUT, PATCH, DELETE)
/// on critical API paths (admin, auth, orders, payments, wallet, subscriptions).
/// Placed after <c>UseAuthorization</c> so that <c>User</c> claims are available.
/// Path matching is version-agnostic: <c>/api/v{any}/resource</c>.
/// </summary>
public sealed partial class AuditLoggingMiddleware
{
    private static readonly HashSet<string> AuditedMethods = new(StringComparer.OrdinalIgnoreCase)
    {
        "POST", "PUT", "PATCH", "DELETE",
    };

    private static readonly HashSet<string> AuditedResourceSegments = new(StringComparer.OrdinalIgnoreCase)
    {
        "admin",
        "auth",
        "orders",
        "payments",
        "wallet",
        "subscriptions",
    };

    [GeneratedRegex(@"^/api/v[^/]+/([^/]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex ApiPathRegex();

    private readonly RequestDelegate _next;
    private readonly ILogger<AuditLoggingMiddleware> _logger;

    public AuditLoggingMiddleware(RequestDelegate next, ILogger<AuditLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        if (!ShouldAudit(context))
            return;

        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
        var role = context.User.FindFirstValue(ClaimTypes.Role) ?? "none";
        var ipAddress = context.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "unknown";

        _logger.LogInformation(
            "AUDIT | {Method} {Path} | User: {UserId} | Role: {Role} | IP: {IpAddress} | Status: {StatusCode}",
            context.Request.Method,
            context.Request.Path.Value,
            userId,
            role,
            ipAddress,
            context.Response.StatusCode);
    }

    private static bool ShouldAudit(HttpContext context)
    {
        if (!AuditedMethods.Contains(context.Request.Method))
            return false;

        var path = context.Request.Path.Value;
        if (string.IsNullOrEmpty(path))
            return false;

        var match = ApiPathRegex().Match(path);
        return match.Success && AuditedResourceSegments.Contains(match.Groups[1].Value);
    }
}
