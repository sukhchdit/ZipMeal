using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Deliveries.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Deliveries.Queries;

internal sealed class GetPartnerDashboardQueryHandler(IAppDbContext db)
    : IRequestHandler<GetPartnerDashboardQuery, Result<PartnerDashboardDto>>
{
    public async Task<Result<PartnerDashboardDto>> Handle(
        GetPartnerDashboardQuery request, CancellationToken ct)
    {
        var location = await db.DeliveryPartnerLocations
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.PartnerId == request.PartnerId, ct);

        var isOnline = location?.IsOnline ?? false;

        var todayStart = DateTimeOffset.UtcNow.Date;

        var totalDeliveries = await db.DeliveryAssignments
            .CountAsync(a => a.PartnerId == request.PartnerId &&
                             a.Status == DeliveryStatus.Delivered, ct);

        var todayDeliveries = await db.DeliveryAssignments
            .CountAsync(a => a.PartnerId == request.PartnerId &&
                             a.Status == DeliveryStatus.Delivered &&
                             a.DeliveredAt >= todayStart, ct);

        var todayEarnings = await db.DeliveryAssignments
            .Where(a => a.PartnerId == request.PartnerId &&
                        a.Status == DeliveryStatus.Delivered &&
                        a.DeliveredAt >= todayStart)
            .SumAsync(a => a.Earnings, ct);

        var totalEarnings = await db.DeliveryAssignments
            .Where(a => a.PartnerId == request.PartnerId &&
                        a.Status == DeliveryStatus.Delivered)
            .SumAsync(a => a.Earnings, ct);

        return Result<PartnerDashboardDto>.Success(new PartnerDashboardDto(
            isOnline, totalDeliveries, todayDeliveries, todayEarnings, totalEarnings));
    }
}
