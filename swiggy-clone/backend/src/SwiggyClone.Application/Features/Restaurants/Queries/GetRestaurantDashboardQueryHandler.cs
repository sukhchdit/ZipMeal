using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;


namespace SwiggyClone.Application.Features.Restaurants.Queries;

internal sealed class GetRestaurantDashboardQueryHandler(IAppDbContext db)
    : IRequestHandler<GetRestaurantDashboardQuery, Result<RestaurantDashboardDto>>
{
    public async Task<Result<RestaurantDashboardDto>> Handle(
        GetRestaurantDashboardQuery request, CancellationToken ct)
    {
        var ownershipResult = await RestaurantOwnershipHelper.VerifyOwnership(
            db, request.RestaurantId, request.OwnerId, ct);

        if (!ownershipResult.IsSuccess)
            return Result<RestaurantDashboardDto>.Failure(ownershipResult.ErrorCode!, ownershipResult.ErrorMessage!);

        var restaurant = ownershipResult.Value;

        var totalOrders = await db.Orders
            .CountAsync(o => o.RestaurantId == request.RestaurantId, ct);

        var pendingOrders = await db.Orders
            .CountAsync(o => o.RestaurantId == request.RestaurantId
                && (o.Status == OrderStatus.Placed || o.Status == OrderStatus.Confirmed), ct);

        var totalMenuItems = await db.MenuItems
            .CountAsync(mi => mi.RestaurantId == request.RestaurantId, ct);

        var activeMenuItems = await db.MenuItems
            .CountAsync(mi => mi.RestaurantId == request.RestaurantId && mi.IsAvailable, ct);

        // Dine-in stats
        var totalTables = await db.RestaurantTables
            .CountAsync(t => t.RestaurantId == request.RestaurantId && t.IsActive, ct);

        var activeTables = await db.RestaurantTables
            .CountAsync(t => t.RestaurantId == request.RestaurantId
                && t.IsActive && t.Status == TableStatus.Occupied, ct);

        var activeSessions = await db.DineInSessions
            .CountAsync(s => s.RestaurantId == request.RestaurantId
                && (s.Status == DineInSessionStatus.Active
                    || s.Status == DineInSessionStatus.BillRequested
                    || s.Status == DineInSessionStatus.PaymentPending), ct);

        var pendingDineInOrders = await db.Orders
            .CountAsync(o => o.RestaurantId == request.RestaurantId
                && o.OrderType == OrderType.DineIn
                && (o.Status == OrderStatus.Placed || o.Status == OrderStatus.Confirmed), ct);

        var dto = new RestaurantDashboardDto(
            totalOrders,
            pendingOrders,
            totalMenuItems,
            activeMenuItems,
            restaurant.AverageRating,
            restaurant.TotalRatings,
            restaurant.IsAcceptingOrders,
            restaurant.Status.ToString(),
            totalTables,
            activeTables,
            activeSessions,
            pendingDineInOrders);

        return Result<RestaurantDashboardDto>.Success(dto);
    }
}
