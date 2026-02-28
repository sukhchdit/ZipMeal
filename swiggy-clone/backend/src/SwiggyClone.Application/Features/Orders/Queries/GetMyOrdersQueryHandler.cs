using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Orders.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Orders.Queries;

internal sealed class GetMyOrdersQueryHandler(IAppDbContext db)
    : IRequestHandler<GetMyOrdersQuery, Result<CursorPagedResult<OrderSummaryDto>>>
{
    public async Task<Result<CursorPagedResult<OrderSummaryDto>>> Handle(
        GetMyOrdersQuery request, CancellationToken ct)
    {
        var query = db.Orders
            .AsNoTracking()
            .Include(o => o.Restaurant)
            .Where(o => o.UserId == request.UserId)
            .OrderByDescending(o => o.CreatedAt);

        // Cursor-based pagination: cursor is the CreatedAt of the last item
        if (request.Cursor is not null && DateTimeOffset.TryParse(request.Cursor, out var cursorDate))
        {
            query = (IOrderedQueryable<Domain.Entities.Order>)query.Where(o => o.CreatedAt < cursorDate);
        }

        var pageSize = Math.Clamp(request.PageSize, 1, 50);
        var orders = await query
            .Take(pageSize + 1)
            .Select(o => new OrderSummaryDto(
                o.Id,
                o.OrderNumber,
                o.Restaurant.Name,
                o.Restaurant.LogoUrl,
                o.Status,
                o.TotalAmount,
                o.Items.Count,
                o.CreatedAt,
                o.ScheduledDeliveryTime))
            .ToListAsync(ct);

        var hasMore = orders.Count > pageSize;
        if (hasMore) orders.RemoveAt(orders.Count - 1);

        var nextCursor = orders.Count > 0 ? orders[^1].CreatedAt.ToString("O") : null;

        return Result<CursorPagedResult<OrderSummaryDto>>.Success(
            new CursorPagedResult<OrderSummaryDto>(orders, nextCursor, null, hasMore, pageSize));
    }
}
