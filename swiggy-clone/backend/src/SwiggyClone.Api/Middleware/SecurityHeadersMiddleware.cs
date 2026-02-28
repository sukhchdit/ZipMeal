namespace SwiggyClone.Api.Middleware;

/// <summary>
/// Adds OWASP-recommended security headers to every HTTP response.
/// Uses <c>Response.OnStarting</c> to inject headers before the response is flushed,
/// following the same pattern as <see cref="CorrelationIdMiddleware"/>.
/// </summary>
public sealed class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            var headers = context.Response.Headers;

            headers["X-Frame-Options"] = "DENY";
            headers["X-Content-Type-Options"] = "nosniff";
            headers["X-XSS-Protection"] = "1; mode=block";
            headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=(self), payment=()";
            headers["Content-Security-Policy"] = "default-src 'none'; frame-ancestors 'none'";

            // Remove headers that disclose server technology
            headers.Remove("Server");
            headers.Remove("X-Powered-By");

            return Task.CompletedTask;
        });

        await _next(context);
    }
}
