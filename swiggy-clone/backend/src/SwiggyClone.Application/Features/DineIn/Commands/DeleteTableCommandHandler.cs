using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.DineIn.Commands;

internal sealed class DeleteTableCommandHandler(IAppDbContext db)
    : IRequestHandler<DeleteTableCommand, Result>
{
    public async Task<Result> Handle(DeleteTableCommand request, CancellationToken ct)
    {
        var ownershipResult = await RestaurantOwnershipHelper.VerifyOwnership(
            db, request.RestaurantId, request.OwnerId, ct);

        if (!ownershipResult.IsSuccess)
            return Result.Failure(ownershipResult.ErrorCode!, ownershipResult.ErrorMessage!);

        var table = await db.RestaurantTables
            .FirstOrDefaultAsync(t => t.Id == request.TableId
                && t.RestaurantId == request.RestaurantId, ct);

        if (table is null)
            return Result.Failure("TABLE_NOT_FOUND", "Table not found.");

        // Reject if table has an active session
        var hasActiveSession = await db.DineInSessions
            .AnyAsync(s => s.TableId == table.Id
                && s.Status == DineInSessionStatus.Active, ct);

        if (hasActiveSession)
            return Result.Failure(
                "TABLE_HAS_ACTIVE_SESSION",
                "Cannot delete a table with an active dine-in session.");

        // Soft-delete
        table.IsActive = false;
        table.Status = TableStatus.Maintenance;
        table.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
