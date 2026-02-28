using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Admin.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Admin.Queries;

internal sealed class GetAdminDashboardQueryHandler(IAppDbContext db)
    : IRequestHandler<GetAdminDashboardQuery, Result<AdminDashboardDto>>
{
    public async Task<Result<AdminDashboardDto>> Handle(
        GetAdminDashboardQuery request, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        var todayStart = now.Date;
        var weekStart = todayStart.AddDays(-(int)todayStart.DayOfWeek);
        var monthStart = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, TimeSpan.Zero);

        // User counts
        var usersQuery = db.Users.AsNoTracking();
        var userCounts = new UserCountsDto(
            Total: await usersQuery.CountAsync(ct),
            Customers: await usersQuery.CountAsync(u => u.Role == UserRole.Customer, ct),
            RestaurantOwners: await usersQuery.CountAsync(u => u.Role == UserRole.RestaurantOwner, ct),
            DeliveryPartners: await usersQuery.CountAsync(u => u.Role == UserRole.DeliveryPartner, ct),
            Admins: await usersQuery.CountAsync(u => u.Role == UserRole.Admin, ct));

        // Restaurant counts
        var restaurantsQuery = db.Restaurants.AsNoTracking();
        var restaurantCounts = new RestaurantCountsDto(
            Total: await restaurantsQuery.CountAsync(ct),
            Pending: await restaurantsQuery.CountAsync(r => r.Status == RestaurantStatus.Pending, ct),
            Approved: await restaurantsQuery.CountAsync(r => r.Status == RestaurantStatus.Approved, ct),
            Suspended: await restaurantsQuery.CountAsync(r => r.Status == RestaurantStatus.Suspended, ct),
            Rejected: await restaurantsQuery.CountAsync(r => r.Status == RestaurantStatus.Rejected, ct));

        // Order counts
        var ordersQuery = db.Orders.AsNoTracking();
        var orderCounts = new OrderCountsDto(
            Today: await ordersQuery.CountAsync(o => o.CreatedAt >= todayStart, ct),
            ThisWeek: await ordersQuery.CountAsync(o => o.CreatedAt >= weekStart, ct),
            ThisMonth: await ordersQuery.CountAsync(o => o.CreatedAt >= monthStart, ct),
            AllTime: await ordersQuery.CountAsync(ct));

        // Revenue (sum of TotalAmount where PaymentStatus == Paid)
        var paidOrders = ordersQuery.Where(o => o.PaymentStatus == PaymentStatus.Paid);
        var revenue = new RevenueDto(
            Today: await paidOrders
                .Where(o => o.CreatedAt >= todayStart)
                .SumAsync(o => (long)o.TotalAmount, ct),
            ThisWeek: await paidOrders
                .Where(o => o.CreatedAt >= weekStart)
                .SumAsync(o => (long)o.TotalAmount, ct),
            ThisMonth: await paidOrders
                .Where(o => o.CreatedAt >= monthStart)
                .SumAsync(o => (long)o.TotalAmount, ct),
            AllTime: await paidOrders
                .SumAsync(o => (long)o.TotalAmount, ct));

        // Recent orders (last 10)
        var recentOrders = await ordersQuery
            .Include(o => o.User)
            .Include(o => o.Restaurant)
            .OrderByDescending(o => o.CreatedAt)
            .Take(10)
            .Select(o => new AdminOrderSummaryDto(
                o.Id,
                o.OrderNumber,
                o.User.FullName,
                o.Restaurant.Name,
                o.Status,
                o.PaymentStatus,
                o.TotalAmount,
                o.CreatedAt))
            .ToListAsync(ct);

        return Result<AdminDashboardDto>.Success(new AdminDashboardDto(
            userCounts, restaurantCounts, orderCounts, revenue, recentOrders));
    }
}
