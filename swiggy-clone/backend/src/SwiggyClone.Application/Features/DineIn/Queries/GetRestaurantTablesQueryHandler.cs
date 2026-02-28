using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.DineIn.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.DineIn.Queries;

internal sealed class GetRestaurantTablesQueryHandler(IAppDbContext db)
    : IRequestHandler<GetRestaurantTablesQuery, Result<List<RestaurantTableDetailDto>>>
{
    public async Task<Result<List<RestaurantTableDetailDto>>> Handle(
        GetRestaurantTablesQuery request, CancellationToken ct)
    {
        var ownershipResult = await RestaurantOwnershipHelper.VerifyOwnership(
            db, request.RestaurantId, request.OwnerId, ct);

        if (!ownershipResult.IsSuccess)
            return Result<List<RestaurantTableDetailDto>>.Failure(
                ownershipResult.ErrorCode!, ownershipResult.ErrorMessage!);

        var tables = await db.RestaurantTables
            .Where(t => t.RestaurantId == request.RestaurantId)
            .OrderBy(t => t.TableNumber)
            .Select(t => new RestaurantTableDetailDto(
                t.Id,
                t.TableNumber,
                t.Capacity,
                t.FloorSection,
                t.QrCodeData,
                t.Status,
                t.IsActive,
                t.DineInSessions.Count(s => s.Status == DineInSessionStatus.Active),
                t.CreatedAt,
                t.UpdatedAt))
            .ToListAsync(ct);

        return Result<List<RestaurantTableDetailDto>>.Success(tables);
    }
}
