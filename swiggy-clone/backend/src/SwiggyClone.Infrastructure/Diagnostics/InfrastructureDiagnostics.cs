using System.Diagnostics;

namespace SwiggyClone.Infrastructure.Diagnostics;

/// <summary>
/// Central registry for Infrastructure-layer tracing.
/// Uses only BCL types (System.Diagnostics) — no OpenTelemetry SDK dependency.
/// </summary>
public static class InfrastructureDiagnostics
{
    public const string ActivitySourceName = "SwiggyClone.Infrastructure";

    public static readonly ActivitySource ActivitySource = new(ActivitySourceName, "1.0.0");
}
