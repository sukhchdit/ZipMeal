using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.DineIn.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.DineIn.Commands;

internal sealed class CreateTableCommandHandler(IAppDbContext db)
    : IRequestHandler<CreateTableCommand, Result<RestaurantTableDetailDto>>
{
    public async Task<Result<RestaurantTableDetailDto>> Handle(
        CreateTableCommand request, CancellationToken ct)
    {
        var ownershipResult = await RestaurantOwnershipHelper.VerifyOwnership(
            db, request.RestaurantId, request.OwnerId, ct);

        if (!ownershipResult.IsSuccess)
            return Result<RestaurantTableDetailDto>.Failure(
                ownershipResult.ErrorCode!, ownershipResult.ErrorMessage!);

        // Check unique table number within restaurant
        var exists = await db.RestaurantTables
            .AnyAsync(t => t.RestaurantId == request.RestaurantId
                && t.TableNumber == request.TableNumber, ct);

        if (exists)
            return Result<RestaurantTableDetailDto>.Failure(
                "TABLE_NUMBER_EXISTS",
                $"Table '{request.TableNumber}' already exists for this restaurant.");

        var now = DateTimeOffset.UtcNow;
        var table = new RestaurantTable
        {
            Id = Guid.CreateVersion7(),
            RestaurantId = request.RestaurantId,
            TableNumber = request.TableNumber,
            Capacity = request.Capacity,
            FloorSection = request.FloorSection,
            QrCodeData = $"DINE-{request.RestaurantId:N}-T{request.TableNumber}",
            Status = TableStatus.Available,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now,
        };

        db.RestaurantTables.Add(table);
        await db.SaveChangesAsync(ct);

        return Result<RestaurantTableDetailDto>.Success(new RestaurantTableDetailDto(
            table.Id,
            table.TableNumber,
            table.Capacity,
            table.FloorSection,
            table.QrCodeData,
            table.Status,
            table.IsActive,
            ActiveSessionCount: 0,
            table.CreatedAt,
            table.UpdatedAt));
    }
}
