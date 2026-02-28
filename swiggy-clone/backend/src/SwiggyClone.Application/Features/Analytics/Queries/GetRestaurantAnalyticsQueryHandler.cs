using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Analytics.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Analytics.Queries;

internal sealed class GetRestaurantAnalyticsQueryHandler(IAppDbContext db)
    : IRequestHandler<GetRestaurantAnalyticsQuery, Result<RestaurantAnalyticsDto>>
{
    public async Task<Result<RestaurantAnalyticsDto>> Handle(
        GetRestaurantAnalyticsQuery request, CancellationToken ct)
    {
        var ownershipResult = await RestaurantOwnershipHelper.VerifyOwnership(
            db, request.RestaurantId, request.OwnerId, ct);

        if (!ownershipResult.IsSuccess)
            return Result<RestaurantAnalyticsDto>.Failure(
                ownershipResult.ErrorCode!, ownershipResult.ErrorMessage!);

        var cutoff = DateBucketHelper.GetCutoff(request.Days);
        var rid = request.RestaurantId;
        var period = request.Period;

        var ordersQuery = db.Orders.AsNoTracking()
            .Where(o => o.RestaurantId == rid && o.CreatedAt >= cutoff);

        // ── Revenue trend ──
        var revenueTrend = await BuildTrend(
            ordersQuery.Where(o => o.PaymentStatus == PaymentStatus.Paid),
            period, true, ct);

        // ── Order count trend ──
        var orderTrend = await BuildTrend(ordersQuery, period, false, ct);

        // ── Top menu items ──
        var topItems = await db.OrderItems.AsNoTracking()
            .Where(oi => oi.Order.RestaurantId == rid && oi.Order.CreatedAt >= cutoff)
            .GroupBy(oi => oi.ItemName)
            .Select(g => new { Name = g.Key, TotalQty = g.Sum(oi => oi.Quantity) })
            .OrderByDescending(x => x.TotalQty)
            .Take(10)
            .ToListAsync(ct);

        var topMenuItems = topItems
            .Select(x => new NamedValueDto(x.Name, x.TotalQty))
            .ToList();

        // ── Rating trend ──
        var ratingTrend = await BuildRatingTrend(rid, cutoff, period, ct);

        // ── Order type distribution ──
        var typeDist = await ordersQuery
            .GroupBy(o => o.OrderType)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var orderTypeDistribution = typeDist
            .Select(x => new NamedValueDto(x.Type.ToString(), x.Count))
            .OrderByDescending(x => x.Value)
            .ToList();

        // ── Order status distribution ──
        var statusDist = await ordersQuery
            .GroupBy(o => o.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var orderStatusDistribution = statusDist
            .Select(x => new NamedValueDto(x.Status.ToString(), x.Count))
            .OrderByDescending(x => x.Value)
            .ToList();

        // ── Peak hours ──
        var peakHours = await ordersQuery
            .GroupBy(o => o.CreatedAt.Hour)
            .Select(g => new { Hour = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var peakHoursDist = Enumerable.Range(0, 24)
            .Select(h =>
            {
                var match = peakHours.FirstOrDefault(x => x.Hour == h);
                return new DataPointDto($"{h:D2}:00", match?.Count ?? 0);
            })
            .ToList();

        // ── Average order value ──
        var paidOrders = ordersQuery.Where(o => o.PaymentStatus == PaymentStatus.Paid);
        var aov = await paidOrders.AnyAsync(ct)
            ? await paidOrders.AverageAsync(o => (decimal)o.TotalAmount, ct)
            : 0m;

        return Result<RestaurantAnalyticsDto>.Success(new RestaurantAnalyticsDto(
            revenueTrend,
            orderTrend,
            topMenuItems,
            ratingTrend,
            orderTypeDistribution,
            orderStatusDistribution,
            peakHoursDist,
            Math.Round(aov, 0)));
    }

    private static async Task<List<DataPointDto>> BuildTrend(
        IQueryable<Domain.Entities.Order> query, string period, bool isRevenue,
        CancellationToken ct)
    {
        return period switch
        {
            "weekly" => (await query
                .GroupBy(o => new { o.CreatedAt.Year, Week = (o.CreatedAt.DayOfYear - 1) / 7 })
                .Select(g => new
                {
                    g.Key.Year, g.Key.Week,
                    Value = isRevenue ? g.Sum(o => (long)o.TotalAmount) : g.Count(),
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Week)
                .ToListAsync(ct))
                .Select(x => new DataPointDto(
                    DateBucketHelper.FormatWeeklyLabel(x.Year, x.Week), x.Value))
                .ToList(),

            "monthly" => (await query
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .Select(g => new
                {
                    g.Key.Year, g.Key.Month,
                    Value = isRevenue ? g.Sum(o => (long)o.TotalAmount) : g.Count(),
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync(ct))
                .Select(x => new DataPointDto(
                    DateBucketHelper.FormatMonthlyLabel(x.Year, x.Month), x.Value))
                .ToList(),

            _ => (await query
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month, o.CreatedAt.Day })
                .Select(g => new
                {
                    g.Key.Year, g.Key.Month, g.Key.Day,
                    Value = isRevenue ? g.Sum(o => (long)o.TotalAmount) : g.Count(),
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month).ThenBy(x => x.Day)
                .ToListAsync(ct))
                .Select(x => new DataPointDto(
                    DateBucketHelper.FormatDailyLabel(x.Year, x.Month, x.Day), x.Value))
                .ToList(),
        };
    }

    private async Task<List<DataPointDto>> BuildRatingTrend(
        Guid restaurantId, DateTimeOffset cutoff, string period, CancellationToken ct)
    {
        var query = db.Reviews.AsNoTracking()
            .Where(r => r.RestaurantId == restaurantId && r.IsVisible && r.CreatedAt >= cutoff);

        return period switch
        {
            "weekly" => (await query
                .GroupBy(r => new { r.CreatedAt.Year, Week = (r.CreatedAt.DayOfYear - 1) / 7 })
                .Select(g => new { g.Key.Year, g.Key.Week, Avg = g.Average(r => (decimal)r.Rating) })
                .OrderBy(x => x.Year).ThenBy(x => x.Week)
                .ToListAsync(ct))
                .Select(x => new DataPointDto(
                    DateBucketHelper.FormatWeeklyLabel(x.Year, x.Week), Math.Round(x.Avg, 1)))
                .ToList(),

            "monthly" => (await query
                .GroupBy(r => new { r.CreatedAt.Year, r.CreatedAt.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Avg = g.Average(r => (decimal)r.Rating) })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync(ct))
                .Select(x => new DataPointDto(
                    DateBucketHelper.FormatMonthlyLabel(x.Year, x.Month), Math.Round(x.Avg, 1)))
                .ToList(),

            _ => (await query
                .GroupBy(r => new { r.CreatedAt.Year, r.CreatedAt.Month, r.CreatedAt.Day })
                .Select(g => new { g.Key.Year, g.Key.Month, g.Key.Day, Avg = g.Average(r => (decimal)r.Rating) })
                .OrderBy(x => x.Year).ThenBy(x => x.Month).ThenBy(x => x.Day)
                .ToListAsync(ct))
                .Select(x => new DataPointDto(
                    DateBucketHelper.FormatDailyLabel(x.Year, x.Month, x.Day), Math.Round(x.Avg, 1)))
                .ToList(),
        };
    }
}
