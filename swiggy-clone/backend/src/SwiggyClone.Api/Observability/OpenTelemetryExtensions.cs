using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SwiggyClone.Application.Common.Diagnostics;
using SwiggyClone.Infrastructure.Diagnostics;

namespace SwiggyClone.Api.Observability;

/// <summary>
/// Configures OpenTelemetry tracing and metrics for the application.
/// </summary>
public static class OpenTelemetryExtensions
{
    public static IServiceCollection AddObservability(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jaegerEndpoint = configuration["OpenTelemetry:JaegerEndpoint"] ?? "http://localhost:4317";

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(
                    serviceName: ApplicationDiagnostics.ServiceName,
                    serviceVersion: "1.0.0"))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation(opts =>
                {
                    opts.RecordException = true;
                    opts.Filter = httpContext =>
                        !httpContext.Request.Path.StartsWithSegments("/health") &&
                        !httpContext.Request.Path.StartsWithSegments("/metrics");
                })
                .AddHttpClientInstrumentation(opts => opts.RecordException = true)
                .AddEntityFrameworkCoreInstrumentation(opts => opts.SetDbStatementForText = true)
                .AddSource(ApplicationDiagnostics.ActivitySourceName)
                .AddSource(InfrastructureDiagnostics.ActivitySourceName)
                .AddOtlpExporter(opts => opts.Endpoint = new Uri(jaegerEndpoint)))
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddProcessInstrumentation()
                .AddMeter(ApplicationDiagnostics.MeterName)
                .AddPrometheusExporter());

        return services;
    }
}
