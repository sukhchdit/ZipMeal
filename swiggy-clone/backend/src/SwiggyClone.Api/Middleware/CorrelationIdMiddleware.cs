using Serilog.Context;
using SwiggyClone.Shared.Constants;

namespace SwiggyClone.Api.Middleware;

/// <summary>
/// Ensures every request/response pair carries a correlation identifier.
/// <list type="bullet">
///   <item>Reads <c>X-Correlation-Id</c> from the incoming request headers.</item>
///   <item>Generates a new <see cref="Guid"/> when the header is absent.</item>
///   <item>Adds the correlation id to the response headers.</item>
///   <item>Pushes a <c>CorrelationId</c> property into the Serilog <see cref="LogContext"/>
///         so that every log entry emitted during the request carries the value.</item>
/// </list>
/// </summary>
public sealed class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrCreateCorrelationId(context);

        // Attach to the response so callers can correlate client-side.
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[AppConstants.CorrelationIdHeader] = correlationId;
            return Task.CompletedTask;
        });

        // Push into Serilog's ambient LogContext.
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(AppConstants.CorrelationIdHeader, out var existing)
            && !string.IsNullOrWhiteSpace(existing))
        {
            return existing.ToString();
        }

        return Guid.NewGuid().ToString("D");
    }
}
