using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace SwiggyClone.Application.Common.Diagnostics;

/// <summary>
/// Central registry for Application-layer metrics and tracing.
/// Uses only BCL types (System.Diagnostics) — no OpenTelemetry SDK dependency.
/// </summary>
public static class ApplicationDiagnostics
{
    public const string ServiceName = "swiggyclone-api";
    public const string MeterName = "SwiggyClone.Application";
    public const string ActivitySourceName = "SwiggyClone.Application";

    public static readonly Meter Meter = new(MeterName, "1.0.0");
    public static readonly ActivitySource ActivitySource = new(ActivitySourceName, "1.0.0");

    // ── Business counters ────────────────────────────────────────────
    public static readonly Counter<long> OrdersPlaced =
        Meter.CreateCounter<long>("swiggyclone.orders.placed", "orders", "Total orders placed");

    public static readonly Counter<long> OrdersCancelled =
        Meter.CreateCounter<long>("swiggyclone.orders.cancelled", "orders", "Total orders cancelled");

    public static readonly Counter<long> PaymentsCompleted =
        Meter.CreateCounter<long>("swiggyclone.payments.completed", "payments", "Total payments completed");

    public static readonly Counter<long> PaymentsFailed =
        Meter.CreateCounter<long>("swiggyclone.payments.failed", "payments", "Total payments failed");

    public static readonly Counter<long> DineInSessionsStarted =
        Meter.CreateCounter<long>("swiggyclone.dinein.sessions_started", "sessions", "Total dine-in sessions started");

    // ── MediatR RED metrics ──────────────────────────────────────────
    public static readonly Counter<long> MediatRRequests =
        Meter.CreateCounter<long>("swiggyclone.mediatr.requests_total", "requests", "Total MediatR requests");

    public static readonly Histogram<double> MediatRDuration =
        Meter.CreateHistogram<double>("swiggyclone.mediatr.duration_seconds", "s", "MediatR request duration in seconds");
}
