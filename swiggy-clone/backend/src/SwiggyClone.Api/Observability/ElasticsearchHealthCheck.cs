using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SwiggyClone.Api.Observability;

/// <summary>
/// Custom Elasticsearch health check that reuses the DI-registered
/// <see cref="ElasticsearchClient"/> to avoid package-version conflicts
/// with the third-party health-check library.
/// </summary>
public sealed class ElasticsearchHealthCheck(ElasticsearchClient client) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await client.PingAsync(cancellationToken);
            return response.IsValidResponse
                ? HealthCheckResult.Healthy("Elasticsearch is reachable")
                : HealthCheckResult.Unhealthy("Elasticsearch ping failed");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Elasticsearch is unreachable", ex);
        }
    }
}
