using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.AbTesting.DTOs;
using SwiggyClone.Shared;
using SwiggyClone.Shared.Constants;

namespace SwiggyClone.Application.Features.AbTesting.Queries.GetExperimentResults;

internal sealed class GetExperimentResultsQueryHandler(
    IAppDbContext db,
    IDistributedCache cache)
    : IRequestHandler<GetExperimentResultsQuery, Result<ExperimentStatsDto>>
{
    private const int MinSampleSize = 30;

    public async Task<Result<ExperimentStatsDto>> Handle(GetExperimentResultsQuery request, CancellationToken ct)
    {
        // 1. Try cache first
        var cacheKey = CacheKeys.ExperimentStats(request.ExperimentId);
        var cached = await cache.GetStringAsync(cacheKey, ct);
        if (cached is not null)
        {
            var cachedStats = JsonSerializer.Deserialize<ExperimentStatsDto>(cached);
            if (cachedStats is not null)
                return Result<ExperimentStatsDto>.Success(cachedStats);
        }

        // 2. Verify experiment exists
        var experiment = await db.Experiments.AsNoTracking()
            .Include(e => e.Variants)
            .FirstOrDefaultAsync(e => e.Id == request.ExperimentId, ct);

        if (experiment is null)
            return Result<ExperimentStatsDto>.Failure("EXPERIMENT_NOT_FOUND", "Experiment not found.");

        // 3. Compute stats
        var stats = await ComputeStatsAsync(request.ExperimentId, experiment.Variants.ToList(), ct);

        // 4. Cache for 30 minutes
        var json = JsonSerializer.Serialize(stats);
        await cache.SetStringAsync(cacheKey, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
        }, ct);

        return Result<ExperimentStatsDto>.Success(stats);
    }

    private async Task<ExperimentStatsDto> ComputeStatsAsync(
        Guid experimentId,
        List<Domain.Entities.ExperimentVariant> variants,
        CancellationToken ct)
    {
        // Get per-variant stats
        var variantStats = new List<VariantStatsDto>();
        VariantRawStats? controlStats = null;
        var rawStatsList = new List<(Domain.Entities.ExperimentVariant Variant, VariantRawStats Stats)>();

        foreach (var variant in variants)
        {
            var exposures = await db.ExposureEvents.AsNoTracking()
                .CountAsync(e => e.Assignment.VariantId == variant.Id, ct);

            var conversions = await db.ConversionEvents.AsNoTracking()
                .CountAsync(e => e.Assignment.VariantId == variant.Id, ct);

            var raw = new VariantRawStats(exposures, conversions);
            rawStatsList.Add((variant, raw));

            if (variant.IsControl)
                controlStats = raw;
        }

        // Compute stats with Z-test
        foreach (var (variant, raw) in rawStatsList)
        {
            var conversionRate = raw.Exposures > 0
                ? (double)raw.Conversions / raw.Exposures
                : 0.0;

            double? relativeLift = null;
            double? zScore = null;
            double? pValue = null;
            bool? isSignificant = null;

            if (!variant.IsControl && controlStats is not null
                && raw.Exposures >= MinSampleSize && controlStats.Exposures >= MinSampleSize)
            {
                var controlRate = (double)controlStats.Conversions / controlStats.Exposures;
                relativeLift = controlRate > 0
                    ? (conversionRate - controlRate) / controlRate
                    : null;

                // Two-proportion Z-test
                var pooledP = (double)(raw.Conversions + controlStats.Conversions)
                    / (raw.Exposures + controlStats.Exposures);

                var se = Math.Sqrt(pooledP * (1 - pooledP)
                    * (1.0 / raw.Exposures + 1.0 / controlStats.Exposures));

                if (se > 0)
                {
                    zScore = (conversionRate - controlRate) / se;
                    pValue = 2.0 * (1.0 - Phi(Math.Abs(zScore.Value)));
                    isSignificant = pValue < 0.05;
                }
            }

            variantStats.Add(new VariantStatsDto(
                variant.Id, variant.Key, variant.Name, variant.IsControl,
                raw.Exposures, raw.Conversions,
                Math.Round(conversionRate, 6),
                relativeLift.HasValue ? Math.Round(relativeLift.Value, 6) : null,
                zScore.HasValue ? Math.Round(zScore.Value, 4) : null,
                pValue.HasValue ? Math.Round(pValue.Value, 6) : null,
                isSignificant));
        }

        return new ExperimentStatsDto(experimentId, variantStats, DateTimeOffset.UtcNow);
    }

    /// <summary>
    /// Standard normal CDF approximation using Horner-method erf.
    /// </summary>
    private static double Phi(double x)
    {
        // Abramowitz and Stegun approximation 7.1.26
        const double a1 = 0.254829592;
        const double a2 = -0.284496736;
        const double a3 = 1.421413741;
        const double a4 = -1.453152027;
        const double a5 = 1.061405429;
        const double p = 0.3275911;

        var sign = x < 0 ? -1 : 1;
        x = Math.Abs(x) / Math.Sqrt(2);

        var t = 1.0 / (1.0 + p * x);
        var y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);

        return 0.5 * (1.0 + sign * y);
    }

    private sealed record VariantRawStats(int Exposures, int Conversions);
}
