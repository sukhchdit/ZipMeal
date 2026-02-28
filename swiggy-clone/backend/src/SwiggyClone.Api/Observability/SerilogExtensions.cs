using System.Diagnostics;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace SwiggyClone.Api.Observability;

/// <summary>
/// Serilog enrichment helpers for OpenTelemetry span correlation
/// and optional Elasticsearch sink.
/// </summary>
public static class SerilogExtensions
{
    /// <summary>
    /// Enriches log events with the current Activity's TraceId and SpanId
    /// for correlation with OpenTelemetry distributed traces.
    /// </summary>
    public static LoggerConfiguration WithSpan(this LoggerEnrichmentConfiguration enrichmentConfiguration)
    {
        return enrichmentConfiguration.With<ActivityEnricher>();
    }

    /// <summary>
    /// Adds Elasticsearch sink for non-Development environments.
    /// </summary>
    public static LoggerConfiguration AddElasticsearchSink(
        this LoggerConfiguration loggerConfiguration,
        IHostEnvironment environment,
        IConfiguration configuration)
    {
        if (!environment.IsDevelopment())
        {
            var elasticUri = configuration["Elasticsearch:Uri"] ?? "http://localhost:9200";
            loggerConfiguration.WriteTo.Elasticsearch(
                new Serilog.Sinks.Elasticsearch.ElasticsearchSinkOptions(new Uri(elasticUri))
                {
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = Serilog.Sinks.Elasticsearch.AutoRegisterTemplateVersion.ESv8,
                    IndexFormat = "swiggyclone-logs-{0:yyyy.MM.dd}",
                });
        }

        return loggerConfiguration;
    }

    private sealed class ActivityEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var activity = Activity.Current;
            if (activity is null)
                return;

            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TraceId", activity.TraceId.ToString()));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("SpanId", activity.SpanId.ToString()));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ParentSpanId", activity.ParentSpanId.ToString()));
        }
    }
}
