using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Analytics.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Analytics.Queries;

internal sealed class GetRestaurantInsightsQueryHandler(IAppDbContext db)
    : IRequestHandler<GetRestaurantInsightsQuery, Result<RestaurantInsightsDto>>
{
    public async Task<Result<RestaurantInsightsDto>> Handle(
        GetRestaurantInsightsQuery request, CancellationToken ct)
    {
        var ownershipResult = await RestaurantOwnershipHelper.VerifyOwnership(
            db, request.RestaurantId, request.OwnerId, ct);

        if (!ownershipResult.IsSuccess)
            return Result<RestaurantInsightsDto>.Failure(
                ownershipResult.ErrorCode!, ownershipResult.ErrorMessage!);

        var cutoff = DateBucketHelper.GetCutoff(request.Days);
        var rid = request.RestaurantId;
        var period = request.Period;

        var ordersQuery = db.Orders.AsNoTracking()
            .Where(o => o.RestaurantId == rid && o.CreatedAt >= cutoff);

        // ── Customer metrics ──
        var userOrderCounts = await ordersQuery
            .GroupBy(o => o.UserId)
            .Select(g => new { UserId = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var newCustomers = userOrderCounts.Count(u => u.Count == 1);
        var repeatCustomers = userOrderCounts.Count(u => u.Count > 1);
        var totalCustomers = userOrderCounts.Count;
        var repeatRate = totalCustomers > 0
            ? Math.Round((double)repeatCustomers / totalCustomers * 100, 1)
            : 0;

        // ── Customer retention trend (distinct customers over time) ──
        var customerRetentionTrend = await BuildCustomerTrend(ordersQuery, period, ct);

        // ── Menu performance ──
        var menuPerformance = await BuildMenuPerformance(rid, cutoff, ct);

        // ── Order efficiency ──
        var totalOrders = await ordersQuery.CountAsync(ct);
        var deliveredCount = await ordersQuery
            .CountAsync(o => o.Status == OrderStatus.Delivered, ct);
        var cancelledCount = await ordersQuery
            .CountAsync(o => o.Status == OrderStatus.Cancelled, ct);

        var completionRate = totalOrders > 0
            ? Math.Round((double)deliveredCount / totalOrders * 100, 1) : 0;
        var cancellationRate = totalOrders > 0
            ? Math.Round((double)cancelledCount / totalOrders * 100, 1) : 0;

        var orderCompletionTrend = await BuildCompletionTrend(ordersQuery, period, ct);

        // ── Revenue by order type ──
        var paidOrders = ordersQuery.Where(o => o.PaymentStatus == PaymentStatus.Paid);

        var revenueByType = await paidOrders
            .GroupBy(o => o.OrderType)
            .Select(g => new { Type = g.Key, Revenue = g.Sum(o => (long)o.TotalAmount) })
            .ToListAsync(ct);

        var revenueByOrderType = revenueByType
            .Select(x => new NamedValueDto(x.Type.ToString(), x.Revenue))
            .OrderByDescending(x => x.Value)
            .ToList();

        // ── Revenue by day of week ──
        var revenueByDow = await paidOrders
            .GroupBy(o => o.CreatedAt.DayOfWeek)
            .Select(g => new { Day = g.Key, Revenue = g.Sum(o => (long)o.TotalAmount) })
            .ToListAsync(ct);

        var revenueByDayOfWeek = Enumerable.Range(0, 7)
            .Select(d =>
            {
                var match = revenueByDow.FirstOrDefault(x => (int)x.Day == d);
                return new NamedValueDto(
                    ((DayOfWeek)d).ToString()[..3], match?.Revenue ?? 0);
            })
            .ToList();

        // ── Avg revenue per customer ──
        var totalRevenue = revenueByType.Sum(x => x.Revenue);
        var avgRevenuePerCustomer = totalCustomers > 0
            ? Math.Round((decimal)totalRevenue / totalCustomers, 0) : 0m;

        return Result<RestaurantInsightsDto>.Success(new RestaurantInsightsDto(
            newCustomers,
            repeatCustomers,
            repeatRate,
            customerRetentionTrend,
            menuPerformance,
            completionRate,
            cancellationRate,
            orderCompletionTrend,
            revenueByOrderType,
            revenueByDayOfWeek,
            avgRevenuePerCustomer));
    }

    private static async Task<List<DataPointDto>> BuildCustomerTrend(
        IQueryable<Domain.Entities.Order> query, string period, CancellationToken ct)
    {
        return period switch
        {
            "weekly" => (await query
                .GroupBy(o => new { o.CreatedAt.Year, Week = (o.CreatedAt.DayOfYear - 1) / 7 })
                .Select(g => new
                {
                    g.Key.Year, g.Key.Week,
                    Value = g.Select(o => o.UserId).Distinct().Count(),
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
                    Value = g.Select(o => o.UserId).Distinct().Count(),
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
                    Value = g.Select(o => o.UserId).Distinct().Count(),
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month).ThenBy(x => x.Day)
                .ToListAsync(ct))
                .Select(x => new DataPointDto(
                    DateBucketHelper.FormatDailyLabel(x.Year, x.Month, x.Day), x.Value))
                .ToList(),
        };
    }

    private async Task<List<MenuItemPerformanceDto>> BuildMenuPerformance(
        Guid restaurantId, DateTimeOffset cutoff, CancellationToken ct)
    {
        var itemStats = await db.OrderItems.AsNoTracking()
            .Where(oi => oi.Order.RestaurantId == restaurantId && oi.Order.CreatedAt >= cutoff)
            .GroupBy(oi => oi.ItemName)
            .Select(g => new
            {
                ItemName = g.Key,
                TotalQuantitySold = g.Sum(oi => oi.Quantity),
                TotalRevenue = g.Sum(oi => (long)oi.TotalPrice),
                OrderCount = g.Select(oi => oi.OrderId).Distinct().Count(),
            })
            .OrderByDescending(x => x.TotalQuantitySold)
            .Take(20)
            .ToListAsync(ct);

        // Get average restaurant rating as fallback for per-item rating
        var avgRating = await db.Reviews.AsNoTracking()
            .Where(r => r.RestaurantId == restaurantId && r.IsVisible && r.CreatedAt >= cutoff)
            .Select(r => (double?)r.Rating)
            .AverageAsync(ct) ?? 0;

        return itemStats
            .Select(x => new MenuItemPerformanceDto(
                x.ItemName,
                x.TotalQuantitySold,
                x.TotalRevenue,
                x.OrderCount,
                Math.Round(avgRating, 1)))
            .ToList();
    }

    private static async Task<List<DataPointDto>> BuildCompletionTrend(
        IQueryable<Domain.Entities.Order> query, string period, CancellationToken ct)
    {
        return period switch
        {
            "weekly" => (await query
                .GroupBy(o => new { o.CreatedAt.Year, Week = (o.CreatedAt.DayOfYear - 1) / 7 })
                .Select(g => new
                {
                    g.Key.Year, g.Key.Week,
                    Total = g.Count(),
                    Delivered = g.Count(o => o.Status == OrderStatus.Delivered),
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Week)
                .ToListAsync(ct))
                .Select(x => new DataPointDto(
                    DateBucketHelper.FormatWeeklyLabel(x.Year, x.Week),
                    x.Total > 0 ? Math.Round((decimal)x.Delivered / x.Total * 100, 1) : 0))
                .ToList(),

            "monthly" => (await query
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .Select(g => new
                {
                    g.Key.Year, g.Key.Month,
                    Total = g.Count(),
                    Delivered = g.Count(o => o.Status == OrderStatus.Delivered),
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync(ct))
                .Select(x => new DataPointDto(
                    DateBucketHelper.FormatMonthlyLabel(x.Year, x.Month),
                    x.Total > 0 ? Math.Round((decimal)x.Delivered / x.Total * 100, 1) : 0))
                .ToList(),

            _ => (await query
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month, o.CreatedAt.Day })
                .Select(g => new
                {
                    g.Key.Year, g.Key.Month, g.Key.Day,
                    Total = g.Count(),
                    Delivered = g.Count(o => o.Status == OrderStatus.Delivered),
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month).ThenBy(x => x.Day)
                .ToListAsync(ct))
                .Select(x => new DataPointDto(
                    DateBucketHelper.FormatDailyLabel(x.Year, x.Month, x.Day),
                    x.Total > 0 ? Math.Round((decimal)x.Delivered / x.Total * 100, 1) : 0))
                .ToList(),
        };
    }
}
