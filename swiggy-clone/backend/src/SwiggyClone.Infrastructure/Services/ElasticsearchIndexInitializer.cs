using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SwiggyClone.Application.Common.Interfaces;

namespace SwiggyClone.Infrastructure.Services;

/// <summary>
/// Background service that ensures ES indices exist on startup.
/// Waits briefly for Elasticsearch to be ready, then creates indices if missing.
/// </summary>
internal sealed class ElasticsearchIndexInitializer(
    ISearchService searchService,
    ILogger<ElasticsearchIndexInitializer> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            // Wait briefly for Elasticsearch to become available
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

            if (!await searchService.IsAvailableAsync(stoppingToken))
            {
                logger.LogWarning("Elasticsearch not available at startup, skipping index initialization");
                return;
            }

            await searchService.EnsureIndicesCreatedAsync(stoppingToken);
            logger.LogInformation("Elasticsearch indices verified/created");
        }
        catch (OperationCanceledException)
        {
            // Shutdown requested, ignore
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize Elasticsearch indices");
        }
    }
}
