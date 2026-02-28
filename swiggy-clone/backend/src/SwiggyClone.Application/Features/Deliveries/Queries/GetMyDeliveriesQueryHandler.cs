using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Deliveries.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Deliveries.Queries;

internal sealed class GetMyDeliveriesQueryHandler(IAppDbContext db)
    : IRequestHandler<GetMyDeliveriesQuery, Result<CursorPagedResult<DeliveryAssignmentDto>>>
{
    public async Task<Result<CursorPagedResult<DeliveryAssignmentDto>>> Handle(
        GetMyDeliveriesQuery request, CancellationToken ct)
    {
        var query = db.DeliveryAssignments
            .AsNoTracking()
            .Where(a => a.PartnerId == request.PartnerId)
            .OrderByDescending(a => a.CreatedAt);

        if (request.Cursor is not null &&
            DateTimeOffset.TryParse(request.Cursor, out var cursorDate))
        {
            query = (IOrderedQueryable<Domain.Entities.DeliveryAssignment>)
                query.Where(a => a.CreatedAt < cursorDate);
        }

        var pageSize = Math.Clamp(request.PageSize, 1, 50);

        var assignments = await query
            .Take(pageSize + 1)
            .AsSplitQuery()
            .Include(a => a.Order)
                .ThenInclude(o => o.Restaurant)
            .Include(a => a.Order)
                .ThenInclude(o => o.DeliveryAddress)
            .Select(a => new DeliveryAssignmentDto(
                a.Id, a.OrderId, a.Order.OrderNumber,
                a.Order.Restaurant.Name,
                a.Order.Restaurant.AddressLine1,
                a.Order.DeliveryAddress != null ? a.Order.DeliveryAddress.AddressLine1 : null,
                (int)a.Status, a.AssignedAt,
                a.AcceptedAt, a.PickedUpAt, a.DeliveredAt,
                a.DistanceKm, a.Earnings))
            .ToListAsync(ct);

        var hasMore = assignments.Count > pageSize;
        if (hasMore) assignments.RemoveAt(assignments.Count - 1);

        var nextCursor = assignments.Count > 0
            ? assignments[^1].AssignedAt.ToString("O")
            : null;

        return Result<CursorPagedResult<DeliveryAssignmentDto>>.Success(
            new CursorPagedResult<DeliveryAssignmentDto>(
                assignments, nextCursor, null, hasMore, pageSize));
    }
}
