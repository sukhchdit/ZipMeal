using System.Security.Claims;

namespace SwiggyClone.Api.Middleware;

/// <summary>
/// Logs a structured audit trail for mutating HTTP methods (POST, PUT, PATCH, DELETE)
/// on critical API paths (admin, auth, orders, payments, wallet, subscriptions).
/// Placed after <c>UseAuthorization</c> so that <c>User</c> claims are available.
/// </summary>
public sealed class AuditLoggingMiddleware
{
    private static readonly HashSet<string> AuditedMethods = new(StringComparer.OrdinalIgnoreCase)
    {
        "POST", "PUT", "PATCH", "DELETE",
    };

    private static readonly string[] AuditedPathPrefixes =
    [
        "/api/v1/admin",
        "/api/v1/auth",
        "/api/v1/orders",
        "/api/v1/payments",
        "/api/v1/wallet",
        "/api/v1/subscriptions",
    ];

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

        foreach (var prefix in AuditedPathPrefixes)
        {
            if (path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
