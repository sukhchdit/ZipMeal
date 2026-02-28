using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.DineIn.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.DineIn.Queries;

internal sealed class GetRestaurantDineInOrdersQueryHandler(IAppDbContext db)
    : IRequestHandler<GetRestaurantDineInOrdersQuery, Result<List<OwnerDineInOrderDto>>>
{
    public async Task<Result<List<OwnerDineInOrderDto>>> Handle(
        GetRestaurantDineInOrdersQuery request, CancellationToken ct)
    {
        var ownershipResult = await RestaurantOwnershipHelper.VerifyOwnership(
            db, request.RestaurantId, request.OwnerId, ct);

        if (!ownershipResult.IsSuccess)
            return Result<List<OwnerDineInOrderDto>>.Failure(
                ownershipResult.ErrorCode!, ownershipResult.ErrorMessage!);

        var query = db.Orders
            .Where(o => o.RestaurantId == request.RestaurantId
                && o.OrderType == OrderType.DineIn);

        // Optional status filter — cast DineInOrderStatus to OrderStatus short value
        if (request.StatusFilter.HasValue)
        {
            var statusShort = (short)request.StatusFilter.Value;
            query = query.Where(o => (short)o.Status == statusShort);
        }

        var orders = await query
            .OrderBy(o => o.Status == OrderStatus.Placed ? 0 :
                          o.Status == OrderStatus.Confirmed ? 1 : 2)
            .ThenByDescending(o => o.CreatedAt)
            .Select(o => new OwnerDineInOrderDto(
                o.Id,
                o.OrderNumber,
                o.DineInTable != null ? o.DineInTable.TableNumber : "N/A",
                o.User.FullName ?? "Guest",
                (DineInOrderStatus)(short)o.Status,
                o.Items.Count,
                o.TotalAmount,
                o.SpecialInstructions,
                o.CreatedAt,
                o.Items.Select(i => new OwnerDineInOrderItemDto(
                    i.ItemName,
                    i.Variant != null ? i.Variant.Name : null,
                    i.Quantity,
                    i.TotalPrice,
                    i.SpecialInstructions)).ToList()))
            .ToListAsync(ct);

        return Result<List<OwnerDineInOrderDto>>.Success(orders);
    }
}
