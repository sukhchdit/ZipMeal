using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.DineIn.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.DineIn.Queries;

internal sealed class GetMyActiveSessionQueryHandler(IAppDbContext db)
    : IRequestHandler<GetMyActiveSessionQuery, Result<DineInSessionSummaryDto?>>
{
    public async Task<Result<DineInSessionSummaryDto?>> Handle(
        GetMyActiveSessionQuery request, CancellationToken ct)
    {
        var summary = await db.DineInSessionMembers
            .AsNoTracking()
            .Where(m => m.UserId == request.UserId
                && m.Session.Status == DineInSessionStatus.Active)
            .Select(m => new DineInSessionSummaryDto(
                m.Session.Id,
                m.Session.RestaurantId,
                m.Session.Restaurant.Name,
                m.Session.Restaurant.LogoUrl,
                m.Session.Table.TableNumber,
                m.Session.SessionCode,
                m.Session.Status,
                m.Session.GuestCount,
                m.Session.StartedAt))
            .FirstOrDefaultAsync(ct);

        return Result<DineInSessionSummaryDto?>.Success(summary);
    }
}
