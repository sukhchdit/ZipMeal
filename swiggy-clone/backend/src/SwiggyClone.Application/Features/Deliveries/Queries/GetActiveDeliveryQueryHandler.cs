using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Deliveries.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Deliveries.Queries;

internal sealed class GetActiveDeliveryQueryHandler(IAppDbContext db)
    : IRequestHandler<GetActiveDeliveryQuery, Result<DeliveryAssignmentDto?>>
{
    public async Task<Result<DeliveryAssignmentDto?>> Handle(
        GetActiveDeliveryQuery request, CancellationToken ct)
    {
        var assignment = await db.DeliveryAssignments
            .AsNoTracking()
            .AsSplitQuery()
            .Where(a => a.PartnerId == request.PartnerId &&
                        a.Status != DeliveryStatus.Delivered &&
                        a.Status != DeliveryStatus.Cancelled)
            .Include(a => a.Order)
                .ThenInclude(o => o.Restaurant)
            .Include(a => a.Order)
                .ThenInclude(o => o.DeliveryAddress)
            .FirstOrDefaultAsync(ct);

        if (assignment is null)
            return Result<DeliveryAssignmentDto?>.Success(null);

        var dto = new DeliveryAssignmentDto(
            assignment.Id, assignment.OrderId, assignment.Order.OrderNumber,
            assignment.Order.Restaurant.Name,
            assignment.Order.Restaurant.AddressLine1,
            assignment.Order.DeliveryAddress?.AddressLine1,
            (int)assignment.Status, assignment.AssignedAt,
            assignment.AcceptedAt, assignment.PickedUpAt, assignment.DeliveredAt,
            assignment.DistanceKm, assignment.Earnings);

        return Result<DeliveryAssignmentDto?>.Success(dto);
    }
}
