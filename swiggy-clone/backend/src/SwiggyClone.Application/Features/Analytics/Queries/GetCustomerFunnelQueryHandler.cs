using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Analytics.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Analytics.Queries;

internal sealed class GetCustomerFunnelQueryHandler(IAppDbContext db)
    : IRequestHandler<GetCustomerFunnelQuery, Result<CustomerFunnelDto>>
{
    public async Task<Result<CustomerFunnelDto>> Handle(
        GetCustomerFunnelQuery request, CancellationToken ct)
    {
        var cutoff = DateBucketHelper.GetCutoff(request.Days);

        // 1. Registered Users (all time)
        var registeredUsers = await db.Users.AsNoTracking().CountAsync(ct);

        // 2. Active Users (interacted or ordered in period)
        var interactionUserIds = db.UserInteractions.AsNoTracking()
            .Where(ui => ui.CreatedAt >= cutoff)
            .Select(ui => ui.UserId);

        var orderUserIds = db.Orders.AsNoTracking()
            .Where(o => o.CreatedAt >= cutoff)
            .Select(o => o.UserId);

        var activeUsers = await interactionUserIds
            .Union(orderUserIds)
            .Distinct()
            .CountAsync(ct);

        // 3. Browsed Restaurants (viewed a restaurant)
        var browsedUsers = await db.UserInteractions.AsNoTracking()
            .Where(ui => ui.CreatedAt >= cutoff
                && ui.EntityType == InteractionEntityType.Restaurant
                && ui.InteractionType == InteractionType.View)
            .Select(ui => ui.UserId)
            .Distinct()
            .CountAsync(ct);

        // 4. Placed Orders
        var placedOrderUsers = await db.Orders.AsNoTracking()
            .Where(o => o.CreatedAt >= cutoff)
            .Select(o => o.UserId)
            .Distinct()
            .CountAsync(ct);

        // 5. Completed Orders (Delivered)
        var completedOrderUsers = await db.Orders.AsNoTracking()
            .Where(o => o.CreatedAt >= cutoff && o.Status == OrderStatus.Delivered)
            .Select(o => o.UserId)
            .Distinct()
            .CountAsync(ct);

        // 6. Left Reviews
        var reviewUsers = await db.Reviews.AsNoTracking()
            .Where(r => r.CreatedAt >= cutoff)
            .Select(r => r.UserId)
            .Distinct()
            .CountAsync(ct);

        // 7. Repeat Customers (>1 order to same restaurant in period)
        var repeatCustomers = await db.Orders.AsNoTracking()
            .Where(o => o.CreatedAt >= cutoff)
            .GroupBy(o => new { o.UserId, o.RestaurantId })
            .Where(g => g.Count() > 1)
            .Select(g => g.Key.UserId)
            .Distinct()
            .CountAsync(ct);

        // Build funnel stages with conversion rates
        var stages = new List<(string Stage, int Count)>
        {
            ("Registered Users", registeredUsers),
            ("Active Users", activeUsers),
            ("Browsed Restaurants", browsedUsers),
            ("Placed Orders", placedOrderUsers),
            ("Completed Orders", completedOrderUsers),
            ("Left Reviews", reviewUsers),
            ("Repeat Customers", repeatCustomers),
        };

        var funnelStages = new List<FunnelStageDto>();
        for (var i = 0; i < stages.Count; i++)
        {
            var (stage, count) = stages[i];
            var previousCount = i == 0 ? count : stages[i - 1].Count;
            var conversionRate = previousCount > 0
                ? Math.Round((double)count / previousCount * 100, 1)
                : 0;
            funnelStages.Add(new FunnelStageDto(stage, count, conversionRate));
        }

        // Active user trend (daily distinct users)
        var activeUserTrend = (await db.Orders.AsNoTracking()
            .Where(o => o.CreatedAt >= cutoff)
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
            .ToList();

        return Result<CustomerFunnelDto>.Success(
            new CustomerFunnelDto(funnelStages, activeUserTrend));
    }
}
