using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Quartz;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Recommendations.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Infrastructure.Persistence;
using SwiggyClone.Shared.Constants;

namespace SwiggyClone.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
internal sealed class PrecomputeRecommendationsJob(
    AppDbContext dbContext,
    IRecommendationEngine engine,
    IDistributedCache cache,
    ILogger<PrecomputeRecommendationsJob> logger) : IJob
{
    private const int BatchSize = 100;
    private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(4);

    public async Task Execute(IJobExecutionContext context)
    {
        var cutoff = DateTimeOffset.UtcNow.AddDays(-90);

        var activeUserIds = await dbContext.Orders
            .AsNoTracking()
            .Where(o => o.Status == OrderStatus.Delivered && o.CreatedAt >= cutoff)
            .Select(o => o.UserId)
            .Distinct()
            .ToListAsync(context.CancellationToken);

        if (activeUserIds.Count == 0)
        {
            return;
        }

        var processedCount = 0;

        for (var i = 0; i < activeUserIds.Count; i += BatchSize)
        {
            var batch = activeUserIds.Skip(i).Take(BatchSize);

            foreach (var userId in batch)
            {
                try
                {
                    var result = await engine.GetPersonalizedAsync(
                        userId, null, ct: context.CancellationToken);

                    var json = JsonSerializer.Serialize(result);
                    await cache.SetStringAsync(
                        CacheKeys.RecommendationsPersonalized(userId),
                        json,
                        new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = CacheTtl },
                        context.CancellationToken);

                    processedCount++;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to precompute recommendations for user {UserId}", userId);
                }
            }

            logger.LogInformation(
                "Precomputed recommendations batch {Batch}/{Total}",
                Math.Min(i + BatchSize, activeUserIds.Count),
                activeUserIds.Count);
        }

        logger.LogInformation(
            "Precomputed recommendations for {Count} active users",
            processedCount);
    }
}
