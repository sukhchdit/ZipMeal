using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SwiggyClone.Api.Observability;

/// <summary>
/// Custom JSON response writer for health check endpoints.
/// Avoids adding the HealthChecks.UI.Client dependency.
/// </summary>
public static class HealthCheckResponseWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    public static async Task WriteResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var result = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.TotalMilliseconds,
            entries = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration.TotalMilliseconds,
                description = e.Value.Description,
                exception = e.Value.Exception?.Message,
                tags = e.Value.Tags,
            }),
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(result, JsonOptions));
    }

    /// <summary>
    /// Creates <see cref="HealthCheckOptions"/> that filter by the "ready" tag
    /// and use the custom JSON writer.
    /// </summary>
    public static HealthCheckOptions ReadinessOptions => new()
    {
        Predicate = check => check.Tags.Contains("ready"),
        ResponseWriter = WriteResponse,
    };
}
