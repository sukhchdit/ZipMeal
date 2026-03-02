using System.Globalization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Analytics.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Analytics.Queries;

internal sealed class GetRevenueForecastQueryHandler(IAppDbContext db)
    : IRequestHandler<GetRevenueForecastQuery, Result<RevenueForecastDto>>
{
    public async Task<Result<RevenueForecastDto>> Handle(
        GetRevenueForecastQuery request, CancellationToken ct)
    {
        // Verify ownership if restaurant-scoped
        if (request.RestaurantId.HasValue && request.OwnerId.HasValue)
        {
            var ownershipResult = await RestaurantOwnershipHelper.VerifyOwnership(
                db, request.RestaurantId.Value, request.OwnerId.Value, ct);

            if (!ownershipResult.IsSuccess)
                return Result<RevenueForecastDto>.Failure(
                    ownershipResult.ErrorCode!, ownershipResult.ErrorMessage!);
        }

        var cutoff = DateBucketHelper.GetCutoff(request.Days);
        var rid = request.RestaurantId;

        // Fetch daily revenue
        var paidOrdersQuery = db.Orders.AsNoTracking()
            .Where(o => o.PaymentStatus == PaymentStatus.Paid && o.CreatedAt >= cutoff);

        if (rid.HasValue)
            paidOrdersQuery = paidOrdersQuery.Where(o => o.RestaurantId == rid.Value);

        var dailyRevenue = await paidOrdersQuery
            .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month, o.CreatedAt.Day })
            .Select(g => new
            {
                g.Key.Year, g.Key.Month, g.Key.Day,
                Revenue = g.Sum(o => (long)o.TotalAmount),
            })
            .OrderBy(x => x.Year).ThenBy(x => x.Month).ThenBy(x => x.Day)
            .ToListAsync(ct);

        // Fill missing days with 0
        var startDate = cutoff.Date;
        var endDate = DateTimeOffset.UtcNow.Date;
        var allDays = new List<(DateOnly Date, decimal Revenue)>();

        for (var d = startDate; d <= endDate; d = d.AddDays(1))
        {
            var match = dailyRevenue.FirstOrDefault(
                x => x.Year == d.Year && x.Month == d.Month && x.Day == d.Day);
            allDays.Add((DateOnly.FromDateTime(d), match?.Revenue ?? 0));
        }

        var historicalData = allDays
            .Select(x => new DataPointDto(
                x.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), x.Revenue))
            .ToList();

        var n = allDays.Count;
        if (n < 2)
        {
            // Edge case: insufficient data → flat projection at average
            var avg = n == 1 ? allDays[0].Revenue : 0m;
            var flatForecast = Enumerable.Range(1, request.ForecastDays)
                .Select(i =>
                {
                    var date = endDate.AddDays(i);
                    return new ForecastPointDto(
                        DateOnly.FromDateTime(date).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                        avg, Math.Max(0, avg - avg * 0.5m), avg + avg * 0.5m);
                })
                .ToList();

            return Result<RevenueForecastDto>.Success(new RevenueForecastDto(
                historicalData, flatForecast,
                avg * request.ForecastDays, avg, 0));
        }

        // Least squares linear regression
        var values = allDays.Select(x => (double)x.Revenue).ToArray();
        var (slope, intercept, standardError) = ComputeLinearRegression(values);

        // Generate forecast
        var forecastData = new List<ForecastPointDto>();
        var margin = 1.96 * standardError;

        for (var i = 1; i <= request.ForecastDays; i++)
        {
            var dayIndex = n + i - 1;
            var predicted = (decimal)(intercept + slope * dayIndex);
            var lower = Math.Max(0, predicted - (decimal)margin);
            var upper = predicted + (decimal)margin;
            predicted = Math.Max(0, predicted);

            var date = endDate.AddDays(i);
            forecastData.Add(new ForecastPointDto(
                DateOnly.FromDateTime(date).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                Math.Round(predicted, 0),
                Math.Round(lower, 0),
                Math.Round(upper, 0)));
        }

        // Summary
        var projectedTotalRevenue = forecastData.Sum(f => f.PredictedValue);
        var avgDailyProjected = request.ForecastDays > 0
            ? Math.Round(projectedTotalRevenue / request.ForecastDays, 0) : 0;
        var historicalAvg = n > 0 ? allDays.Average(x => x.Revenue) : 0m;
        var growthRate = historicalAvg > 0
            ? Math.Round((double)(avgDailyProjected - historicalAvg) / (double)historicalAvg * 100, 1)
            : 0;

        return Result<RevenueForecastDto>.Success(new RevenueForecastDto(
            historicalData, forecastData,
            Math.Round(projectedTotalRevenue, 0),
            avgDailyProjected,
            growthRate));
    }

    private static (double Slope, double Intercept, double StandardError) ComputeLinearRegression(
        double[] values)
    {
        var n = values.Length;
        double sumX = 0, sumY = 0, sumXy = 0, sumX2 = 0;

        for (var i = 0; i < n; i++)
        {
            sumX += i;
            sumY += values[i];
            sumXy += i * values[i];
            sumX2 += (double)i * i;
        }

        var denominator = n * sumX2 - sumX * sumX;
        if (denominator == 0)
            return (0, sumY / n, 0);

        var slope = (n * sumXy - sumX * sumY) / denominator;
        var intercept = (sumY - slope * sumX) / n;

        // Standard error of the estimate
        var sumResidualsSq = 0.0;
        for (var i = 0; i < n; i++)
        {
            var predicted = intercept + slope * i;
            var residual = values[i] - predicted;
            sumResidualsSq += residual * residual;
        }

        var standardError = n > 2 ? Math.Sqrt(sumResidualsSq / (n - 2)) : 0;

        return (slope, intercept, standardError);
    }
}
