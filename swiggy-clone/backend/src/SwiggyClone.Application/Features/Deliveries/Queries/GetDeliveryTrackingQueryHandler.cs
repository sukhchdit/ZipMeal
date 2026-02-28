using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Deliveries.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Deliveries.Queries;

internal sealed class GetDeliveryTrackingQueryHandler(IAppDbContext db)
    : IRequestHandler<GetDeliveryTrackingQuery, Result<DeliveryTrackingDto>>
{
    public async Task<Result<DeliveryTrackingDto>> Handle(
        GetDeliveryTrackingQuery request, CancellationToken ct)
    {
        var order = await db.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.UserId == request.UserId, ct);

        if (order is null)
            return Result<DeliveryTrackingDto>.Failure("ORDER_NOT_FOUND", "Order not found.");

        var assignment = await db.DeliveryAssignments
            .AsNoTracking()
            .Where(a => a.OrderId == request.OrderId &&
                        a.Status != DeliveryStatus.Cancelled)
            .Include(a => a.Partner)
            .OrderByDescending(a => a.AssignedAt)
            .FirstOrDefaultAsync(ct);

        if (assignment is null)
            return Result<DeliveryTrackingDto>.Failure(
                "NO_DELIVERY_ASSIGNMENT", "No delivery assignment found for this order.");

        return Result<DeliveryTrackingDto>.Success(new DeliveryTrackingDto(
            order.Id,
            (int)assignment.Status,
            assignment.Partner.FullName,
            assignment.Partner.PhoneNumber,
            assignment.CurrentLatitude,
            assignment.CurrentLongitude,
            assignment.AssignedAt,
            assignment.AcceptedAt,
            assignment.PickedUpAt,
            order.EstimatedDeliveryTime));
    }
}
