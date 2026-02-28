using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Queries;

internal sealed class GetOperatingHoursQueryHandler(IAppDbContext db)
    : IRequestHandler<GetOperatingHoursQuery, Result<List<OperatingHoursDto>>>
{
    public async Task<Result<List<OperatingHoursDto>>> Handle(
        GetOperatingHoursQuery request, CancellationToken ct)
    {
        var ownershipResult = await RestaurantOwnershipHelper.VerifyOwnership(
            db, request.RestaurantId, request.OwnerId, ct);

        if (!ownershipResult.IsSuccess)
            return Result<List<OperatingHoursDto>>.Failure(ownershipResult.ErrorCode!, ownershipResult.ErrorMessage!);

        var hours = await db.RestaurantOperatingHours
            .AsNoTracking()
            .Where(h => h.RestaurantId == request.RestaurantId)
            .OrderBy(h => h.DayOfWeek)
            .Select(h => new OperatingHoursDto(
                h.Id,
                h.DayOfWeek,
                h.OpenTime,
                h.CloseTime,
                h.IsClosed))
            .ToListAsync(ct);

        return Result<List<OperatingHoursDto>>.Success(hours);
    }
}
