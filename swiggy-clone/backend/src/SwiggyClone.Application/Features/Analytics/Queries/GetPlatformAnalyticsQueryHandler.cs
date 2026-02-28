using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Analytics.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Analytics.Queries;

internal sealed class GetPlatformAnalyticsQueryHandler(IAppDbContext db)
    : IRequestHandler<GetPlatformAnalyticsQuery, Result<PlatformAnalyticsDto>>
{
    public async Task<Result<PlatformAnalyticsDto>> Handle(
        GetPlatformAnalyticsQuery request, CancellationToken ct)
    {
        var cutoff = DateBucketHelper.GetCutoff(request.Days);
        var period = request.Period;

        // ── Revenue trend ──
        var revenueTrend = await BuildOrderTrend(
            cutoff, period, true, ct);

        // ── Order count trend ──
        var orderTrend = await BuildOrderTrend(
            cutoff, period, false, ct);

        // ── User growth trend ──
        var userGrowthTrend = await BuildUserGrowthTrend(cutoff, period, ct);

        // ── Order status distribution ──
        var orderStatusDistribution = await db.Orders.AsNoTracking()
            .Where(o => o.CreatedAt >= cutoff)
            .GroupBy(o => o.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var statusDist = orderStatusDistribution
            .Select(x => new NamedValueDto(x.Status.ToString(), x.Count))
            .OrderByDescending(x => x.Value)
            .ToList();

        // ── Order type distribution ──
        var orderTypeDistribution = await db.Orders.AsNoTracking()
            .Where(o => o.CreatedAt >= cutoff)
            .GroupBy(o => o.OrderType)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var typeDist = orderTypeDistribution
            .Select(x => new NamedValueDto(x.Type.ToString(), x.Count))
            .OrderByDescending(x => x.Value)
            .ToList();

        // ── Payment method distribution ──
        var paymentMethodDistribution = await db.Orders.AsNoTracking()
            .Where(o => o.CreatedAt >= cutoff && o.PaymentStatus == PaymentStatus.Paid)
            .GroupBy(o => o.PaymentMethod)
            .Select(g => new { Method = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var paymentDist = paymentMethodDistribution
            .Select(x => new NamedValueDto(x.Method?.ToString() ?? "Unknown", x.Count))
            .OrderByDescending(x => x.Value)
            .ToList();

        // ── Top restaurants by revenue ──
        var topByRevenue = await db.Orders.AsNoTracking()
            .Where(o => o.CreatedAt >= cutoff && o.PaymentStatus == PaymentStatus.Paid)
            .GroupBy(o => new { o.RestaurantId, o.Restaurant.Name })
            .Select(g => new { g.Key.Name, Revenue = g.Sum(o => (long)o.TotalAmount) })
            .OrderByDescending(x => x.Revenue)
            .Take(10)
            .ToListAsync(ct);

        var topRevenue = topByRevenue
            .Select(x => new NamedValueDto(x.Name, x.Revenue))
            .ToList();

        // ── Top restaurants by orders ──
        var topByOrders = await db.Orders.AsNoTracking()
            .Where(o => o.CreatedAt >= cutoff)
            .GroupBy(o => new { o.RestaurantId, o.Restaurant.Name })
            .Select(g => new { g.Key.Name, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync(ct);

        var topOrders = topByOrders
            .Select(x => new NamedValueDto(x.Name, x.Count))
            .ToList();

        // ── Popular menu items ──
        var popularItems = await db.OrderItems.AsNoTracking()
            .Where(oi => oi.Order.CreatedAt >= cutoff)
            .GroupBy(oi => oi.ItemName)
            .Select(g => new { Name = g.Key, TotalQty = g.Sum(oi => oi.Quantity) })
            .OrderByDescending(x => x.TotalQty)
            .Take(10)
            .ToListAsync(ct);

        var popularMenuItems = popularItems
            .Select(x => new NamedValueDto(x.Name, x.TotalQty))
            .ToList();

        // ── Coupon stats ──
        var couponStats = await BuildCouponStats(cutoff, ct);

        // ── Delivery performance ──
        var deliveryPerformance = await BuildDeliveryPerformance(cutoff, ct);

        // ── Peak hours distribution ──
        var peakHours = await db.Orders.AsNoTracking()
            .Where(o => o.CreatedAt >= cutoff)
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

        return Result<PlatformAnalyticsDto>.Success(new PlatformAnalyticsDto(
            revenueTrend,
            orderTrend,
            userGrowthTrend,
            statusDist,
            typeDist,
            paymentDist,
            topRevenue,
            topOrders,
            popularMenuItems,
            couponStats,
            deliveryPerformance,
            peakHoursDist));
    }

    private async Task<List<DataPointDto>> BuildOrderTrend(
        DateTimeOffset cutoff, string period, bool isRevenue, CancellationToken ct)
    {
        var query = db.Orders.AsNoTracking()
            .Where(o => o.CreatedAt >= cutoff);

        if (isRevenue)
            query = query.Where(o => o.PaymentStatus == PaymentStatus.Paid);

        return period switch
        {
            "weekly" => await BuildWeeklyOrderTrend(query, isRevenue, ct),
            "monthly" => await BuildMonthlyOrderTrend(query, isRevenue, ct),
            _ => await BuildDailyOrderTrend(query, isRevenue, ct),
        };
    }

    private static async Task<List<DataPointDto>> BuildDailyOrderTrend(
        IQueryable<Domain.Entities.Order> query, bool isRevenue, CancellationToken ct)
    {
        var grouped = await query
            .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month, o.CreatedAt.Day })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                g.Key.Day,
                Value = isRevenue ? g.Sum(o => (long)o.TotalAmount) : g.Count(),
            })
            .OrderBy(x => x.Year).ThenBy(x => x.Month).ThenBy(x => x.Day)
            .ToListAsync(ct);

        return grouped
            .Select(x => new DataPointDto(
                DateBucketHelper.FormatDailyLabel(x.Year, x.Month, x.Day), x.Value))
            .ToList();
    }

    private static async Task<List<DataPointDto>> BuildWeeklyOrderTrend(
        IQueryable<Domain.Entities.Order> query, bool isRevenue, CancellationToken ct)
    {
        var grouped = await query
            .GroupBy(o => new { o.CreatedAt.Year, Week = (o.CreatedAt.DayOfYear - 1) / 7 })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Week,
                Value = isRevenue ? g.Sum(o => (long)o.TotalAmount) : g.Count(),
            })
            .OrderBy(x => x.Year).ThenBy(x => x.Week)
            .ToListAsync(ct);

        return grouped
            .Select(x => new DataPointDto(
                DateBucketHelper.FormatWeeklyLabel(x.Year, x.Week), x.Value))
            .ToList();
    }

    private static async Task<List<DataPointDto>> BuildMonthlyOrderTrend(
        IQueryable<Domain.Entities.Order> query, bool isRevenue, CancellationToken ct)
    {
        var grouped = await query
            .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                Value = isRevenue ? g.Sum(o => (long)o.TotalAmount) : g.Count(),
            })
            .OrderBy(x => x.Year).ThenBy(x => x.Month)
            .ToListAsync(ct);

        return grouped
            .Select(x => new DataPointDto(
                DateBucketHelper.FormatMonthlyLabel(x.Year, x.Month), x.Value))
            .ToList();
    }

    private async Task<List<DataPointDto>> BuildUserGrowthTrend(
        DateTimeOffset cutoff, string period, CancellationToken ct)
    {
        var query = db.Users.AsNoTracking().Where(u => u.CreatedAt >= cutoff);

        return period switch
        {
            "weekly" => (await query
                .GroupBy(u => new { u.CreatedAt.Year, Week = (u.CreatedAt.DayOfYear - 1) / 7 })
                .Select(g => new { g.Key.Year, g.Key.Week, Count = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Week)
                .ToListAsync(ct))
                .Select(x => new DataPointDto(
                    DateBucketHelper.FormatWeeklyLabel(x.Year, x.Week), x.Count))
                .ToList(),
            "monthly" => (await query
                .GroupBy(u => new { u.CreatedAt.Year, u.CreatedAt.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync(ct))
                .Select(x => new DataPointDto(
                    DateBucketHelper.FormatMonthlyLabel(x.Year, x.Month), x.Count))
                .ToList(),
            _ => (await query
                .GroupBy(u => new { u.CreatedAt.Year, u.CreatedAt.Month, u.CreatedAt.Day })
                .Select(g => new { g.Key.Year, g.Key.Month, g.Key.Day, Count = g.Count() })
                .OrderBy(x => x.Year).ThenBy(x => x.Month).ThenBy(x => x.Day)
                .ToListAsync(ct))
                .Select(x => new DataPointDto(
                    DateBucketHelper.FormatDailyLabel(x.Year, x.Month, x.Day), x.Count))
                .ToList(),
        };
    }

    private async Task<CouponAnalyticsDto> BuildCouponStats(
        DateTimeOffset cutoff, CancellationToken ct)
    {
        var usagesQuery = db.CouponUsages.AsNoTracking()
            .Where(cu => cu.UsedAt >= cutoff);

        var totalUsed = await usagesQuery.CountAsync(ct);
        var totalDiscount = totalUsed > 0
            ? await usagesQuery.SumAsync(cu => (long)cu.DiscountAmount, ct)
            : 0L;
        var uniqueCoupons = await usagesQuery
            .Select(cu => cu.CouponId).Distinct().CountAsync(ct);

        var topCoupons = await usagesQuery
            .GroupBy(cu => new { cu.CouponId, cu.Coupon.Code })
            .Select(g => new { g.Key.Code, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToListAsync(ct);

        return new CouponAnalyticsDto(
            totalUsed,
            totalDiscount,
            uniqueCoupons,
            topCoupons.Select(x => new NamedValueDto(x.Code, x.Count)).ToList());
    }

    private async Task<DeliveryPerformanceDto> BuildDeliveryPerformance(
        DateTimeOffset cutoff, CancellationToken ct)
    {
        var query = db.DeliveryAssignments.AsNoTracking()
            .Where(da => da.CreatedAt >= cutoff);

        var total = await query.CountAsync(ct);
        var delivered = await query
            .CountAsync(da => da.Status == DeliveryStatus.Delivered, ct);
        var cancelled = await query
            .CountAsync(da => da.Status == DeliveryStatus.Cancelled, ct);

        var completionRate = total > 0 ? (double)delivered / total * 100 : 0;

        // Avg delivery time: PickedUpAt to DeliveredAt for completed deliveries
        var avgMinutes = 0.0;
        var completedWithTimes = await query
            .Where(da => da.Status == DeliveryStatus.Delivered
                && da.PickedUpAt != null && da.DeliveredAt != null)
            .Select(da => new
            {
                PickedUp = da.PickedUpAt!.Value,
                Delivered = da.DeliveredAt!.Value,
            })
            .ToListAsync(ct);

        if (completedWithTimes.Count > 0)
        {
            avgMinutes = completedWithTimes
                .Average(x => (x.Delivered - x.PickedUp).TotalMinutes);
        }

        return new DeliveryPerformanceDto(
            Math.Round(avgMinutes, 1),
            Math.Round(completionRate, 1),
            total,
            cancelled);
    }
}
