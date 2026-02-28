using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.DineIn.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.DineIn.Queries;

internal sealed class GetRestaurantSessionsQueryHandler(IAppDbContext db)
    : IRequestHandler<GetRestaurantSessionsQuery, Result<List<OwnerSessionDto>>>
{
    public async Task<Result<List<OwnerSessionDto>>> Handle(
        GetRestaurantSessionsQuery request, CancellationToken ct)
    {
        var ownershipResult = await RestaurantOwnershipHelper.VerifyOwnership(
            db, request.RestaurantId, request.OwnerId, ct);

        if (!ownershipResult.IsSuccess)
            return Result<List<OwnerSessionDto>>.Failure(
                ownershipResult.ErrorCode!, ownershipResult.ErrorMessage!);

        var sessions = await db.DineInSessions
            .Where(s => s.RestaurantId == request.RestaurantId
                && (s.Status == DineInSessionStatus.Active
                    || s.Status == DineInSessionStatus.BillRequested
                    || s.Status == DineInSessionStatus.PaymentPending))
            .OrderByDescending(s => s.StartedAt)
            .Select(s => new OwnerSessionDto(
                s.Id,
                s.Table.TableNumber,
                s.Table.FloorSection,
                s.SessionCode,
                s.Status,
                s.GuestCount,
                s.Members.Count,
                s.Orders.Count,
                s.Orders.Sum(o => o.TotalAmount),
                s.StartedAt,
                s.EndedAt))
            .ToListAsync(ct);

        return Result<List<OwnerSessionDto>>.Success(sessions);
    }
}
