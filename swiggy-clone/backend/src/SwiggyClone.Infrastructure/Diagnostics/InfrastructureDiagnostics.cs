using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace SwiggyClone.Infrastructure.Diagnostics;

/// <summary>
/// Central registry for Infrastructure-layer tracing and metrics.
/// Uses only BCL types (System.Diagnostics) — no OpenTelemetry SDK dependency.
/// </summary>
public static class InfrastructureDiagnostics
{
    public const string ActivitySourceName = "SwiggyClone.Infrastructure";
    public const string MeterName = "SwiggyClone.Infrastructure";

    public static readonly ActivitySource ActivitySource = new(ActivitySourceName, "1.0.0");
    public static readonly Meter Meter = new(MeterName, "1.0.0");

    // ── Resilience counters ──────────────────────────────────────────
    public static readonly Counter<long> ResilienceRetries =
        Meter.CreateCounter<long>(
            "swiggyclone.resilience.retries_total",
            "retries",
            "Total resilience retry attempts");

    public static readonly Counter<long> CircuitBreakerStateChanges =
        Meter.CreateCounter<long>(
            "swiggyclone.resilience.circuit_breaker_state_changes_total",
            "changes",
            "Total circuit breaker state changes");

    public static readonly Counter<long> ResilienceTimeouts =
        Meter.CreateCounter<long>(
            "swiggyclone.resilience.timeouts_total",
            "timeouts",
            "Total resilience timeout occurrences");
}
