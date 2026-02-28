using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.DineIn.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.DineIn.Commands;

internal sealed class UpdateTableCommandHandler(IAppDbContext db)
    : IRequestHandler<UpdateTableCommand, Result<RestaurantTableDetailDto>>
{
    public async Task<Result<RestaurantTableDetailDto>> Handle(
        UpdateTableCommand request, CancellationToken ct)
    {
        var ownershipResult = await RestaurantOwnershipHelper.VerifyOwnership(
            db, request.RestaurantId, request.OwnerId, ct);

        if (!ownershipResult.IsSuccess)
            return Result<RestaurantTableDetailDto>.Failure(
                ownershipResult.ErrorCode!, ownershipResult.ErrorMessage!);

        var table = await db.RestaurantTables
            .FirstOrDefaultAsync(t => t.Id == request.TableId
                && t.RestaurantId == request.RestaurantId, ct);

        if (table is null)
            return Result<RestaurantTableDetailDto>.Failure(
                "TABLE_NOT_FOUND", "Table not found.");

        // Check for active sessions if deactivating or setting maintenance
        if ((request.IsActive == false || request.Status == TableStatus.Maintenance)
            && table.Status == TableStatus.Occupied)
        {
            var hasActiveSession = await db.DineInSessions
                .AnyAsync(s => s.TableId == table.Id
                    && s.Status == DineInSessionStatus.Active, ct);

            if (hasActiveSession)
                return Result<RestaurantTableDetailDto>.Failure(
                    "TABLE_HAS_ACTIVE_SESSION",
                    "Cannot deactivate or set maintenance on a table with an active session.");
        }

        // If changing table number, check uniqueness
        if (request.TableNumber is not null && request.TableNumber != table.TableNumber)
        {
            var numberExists = await db.RestaurantTables
                .AnyAsync(t => t.RestaurantId == request.RestaurantId
                    && t.TableNumber == request.TableNumber
                    && t.Id != table.Id, ct);

            if (numberExists)
                return Result<RestaurantTableDetailDto>.Failure(
                    "TABLE_NUMBER_EXISTS",
                    $"Table '{request.TableNumber}' already exists for this restaurant.");

            table.TableNumber = request.TableNumber;
            table.QrCodeData = $"DINE-{request.RestaurantId:N}-T{request.TableNumber}";
        }

        if (request.Capacity.HasValue)
            table.Capacity = request.Capacity.Value;

        if (request.FloorSection is not null)
            table.FloorSection = request.FloorSection;

        if (request.IsActive.HasValue)
            table.IsActive = request.IsActive.Value;

        if (request.Status.HasValue)
            table.Status = request.Status.Value;

        table.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct);

        var activeSessionCount = await db.DineInSessions
            .CountAsync(s => s.TableId == table.Id
                && s.Status == DineInSessionStatus.Active, ct);

        return Result<RestaurantTableDetailDto>.Success(new RestaurantTableDetailDto(
            table.Id,
            table.TableNumber,
            table.Capacity,
            table.FloorSection,
            table.QrCodeData,
            table.Status,
            table.IsActive,
            activeSessionCount,
            table.CreatedAt,
            table.UpdatedAt));
    }
}
