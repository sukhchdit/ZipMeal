using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Quartz;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Infrastructure.Persistence;
using SwiggyClone.Shared.Constants;

namespace SwiggyClone.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
internal sealed class ComputeExperimentStatsJob(
    AppDbContext dbContext,
    IDistributedCache cache,
    ILogger<ComputeExperimentStatsJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var ct = context.CancellationToken;

        var activeExperiments = await dbContext.Experiments.AsNoTracking()
            .Include(e => e.Variants)
            .Where(e => e.Status == ExperimentStatus.Active || e.Status == ExperimentStatus.Completed)
            .ToListAsync(ct);

        foreach (var experiment in activeExperiments)
        {
            var variantStats = new List<object>();

            foreach (var variant in experiment.Variants)
            {
                var exposures = await dbContext.ExposureEvents.AsNoTracking()
                    .CountAsync(e => e.Assignment.VariantId == variant.Id, ct);

                var conversions = await dbContext.ConversionEvents.AsNoTracking()
                    .CountAsync(e => e.Assignment.VariantId == variant.Id, ct);

                var conversionRate = exposures > 0 ? (double)conversions / exposures : 0.0;

                variantStats.Add(new
                {
                    VariantId = variant.Id,
                    VariantKey = variant.Key,
                    VariantName = variant.Name,
                    variant.IsControl,
                    Exposures = exposures,
                    Conversions = conversions,
                    ConversionRate = Math.Round(conversionRate, 6),
                    RelativeLift = (double?)null,
                    ZScore = (double?)null,
                    PValue = (double?)null,
                    IsSignificant = (bool?)null,
                });
            }

            var stats = new
            {
                ExperimentId = experiment.Id,
                Variants = variantStats,
                ComputedAt = DateTimeOffset.UtcNow,
            };

            var cacheKey = CacheKeys.ExperimentStats(experiment.Id);
            var json = JsonSerializer.Serialize(stats);
            await cache.SetStringAsync(cacheKey, json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(35),
            }, ct);
        }

        logger.LogInformation("Precomputed stats for {Count} experiments", activeExperiments.Count);
    }
}
