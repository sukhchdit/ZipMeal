using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Analytics.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Analytics.Queries;

internal sealed class GetPartnerAnalyticsQueryHandler(IAppDbContext db)
    : IRequestHandler<GetPartnerAnalyticsQuery, Result<PartnerAnalyticsDto>>
{
    public async Task<Result<PartnerAnalyticsDto>> Handle(
        GetPartnerAnalyticsQuery request, CancellationToken ct)
    {
        var cutoff = DateBucketHelper.GetCutoff(request.Days);
        var pid = request.PartnerId;
        var period = request.Period;

        var query = db.DeliveryAssignments.AsNoTracking()
            .Where(da => da.PartnerId == pid && da.CreatedAt >= cutoff);

        // ── Earnings trend ──
        var earningsTrend = await BuildDeliveryTrend(
            query.Where(da => da.Status == DeliveryStatus.Delivered),
            period, true, ct);

        // ── Delivery count trend ──
        var deliveryCountTrend = await BuildDeliveryTrend(
            query.Where(da => da.Status == DeliveryStatus.Delivered),
            period, false, ct);

        // ── Aggregate stats ──
        var total = await query.CountAsync(ct);
        var delivered = await query
            .CountAsync(da => da.Status == DeliveryStatus.Delivered, ct);
        var completionRate = total > 0 ? (double)delivered / total * 100 : 0;

        var totalEarnings = delivered > 0
            ? await query
                .Where(da => da.Status == DeliveryStatus.Delivered)
                .SumAsync(da => (long)da.Earnings, ct)
            : 0L;

        // ── Average delivery time ──
        var completedWithTimes = await query
            .Where(da => da.Status == DeliveryStatus.Delivered
                && da.PickedUpAt != null && da.DeliveredAt != null)
            .Select(da => new
            {
                PickedUp = da.PickedUpAt!.Value,
                Delivered = da.DeliveredAt!.Value,
            })
            .ToListAsync(ct);

        var avgMinutes = completedWithTimes.Count > 0
            ? completedWithTimes.Average(x => (x.Delivered - x.PickedUp).TotalMinutes)
            : 0.0;

        // ── Average delivery rating ──
        var avgRating = await db.Reviews.AsNoTracking()
            .Where(r => r.Order.DeliveryPartnerId == pid
                && r.DeliveryRating != null
                && r.CreatedAt >= cutoff)
            .Select(r => (double?)r.DeliveryRating)
            .AverageAsync(ct) ?? 0.0;

        return Result<PartnerAnalyticsDto>.Success(new PartnerAnalyticsDto(
            earningsTrend,
            deliveryCountTrend,
            Math.Round(avgMinutes, 1),
            Math.Round(completionRate, 1),
            total,
            totalEarnings,
            Math.Round(avgRating, 1)));
    }

    private static async Task<List<DataPointDto>> BuildDeliveryTrend(
        IQueryable<Domain.Entities.DeliveryAssignment> query, string period,
        bool isEarnings, CancellationToken ct)
    {
        return period switch
        {
            "weekly" => (await query
                .GroupBy(da => new { da.CreatedAt.Year, Week = (da.CreatedAt.DayOfYear - 1) / 7 })
                .Select(g => new
                {
                    g.Key.Year, g.Key.Week,
                    Value = isEarnings ? g.Sum(da => (long)da.Earnings) : g.Count(),
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Week)
                .ToListAsync(ct))
                .Select(x => new DataPointDto(
                    DateBucketHelper.FormatWeeklyLabel(x.Year, x.Week), x.Value))
                .ToList(),

            "monthly" => (await query
                .GroupBy(da => new { da.CreatedAt.Year, da.CreatedAt.Month })
                .Select(g => new
                {
                    g.Key.Year, g.Key.Month,
                    Value = isEarnings ? g.Sum(da => (long)da.Earnings) : g.Count(),
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync(ct))
                .Select(x => new DataPointDto(
                    DateBucketHelper.FormatMonthlyLabel(x.Year, x.Month), x.Value))
                .ToList(),

            _ => (await query
                .GroupBy(da => new { da.CreatedAt.Year, da.CreatedAt.Month, da.CreatedAt.Day })
                .Select(g => new
                {
                    g.Key.Year, g.Key.Month, g.Key.Day,
                    Value = isEarnings ? g.Sum(da => (long)da.Earnings) : g.Count(),
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month).ThenBy(x => x.Day)
                .ToListAsync(ct))
                .Select(x => new DataPointDto(
                    DateBucketHelper.FormatDailyLabel(x.Year, x.Month, x.Day), x.Value))
                .ToList(),
        };
    }
}
