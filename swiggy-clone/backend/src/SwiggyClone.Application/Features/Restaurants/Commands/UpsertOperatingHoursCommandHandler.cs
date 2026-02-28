using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Restaurants.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

internal sealed class UpsertOperatingHoursCommandHandler(IAppDbContext db)
    : IRequestHandler<UpsertOperatingHoursCommand, Result<List<OperatingHoursDto>>>
{
    public async Task<Result<List<OperatingHoursDto>>> Handle(
        UpsertOperatingHoursCommand request, CancellationToken ct)
    {
        var ownershipResult = await RestaurantOwnershipHelper.VerifyOwnership(
            db, request.RestaurantId, request.OwnerId, ct);

        if (ownershipResult.IsFailure)
            return Result<List<OperatingHoursDto>>.Failure(ownershipResult.ErrorCode!, ownershipResult.ErrorMessage!);

        var existingHours = await db.RestaurantOperatingHours
            .Where(h => h.RestaurantId == request.RestaurantId)
            .ToListAsync(ct);

        foreach (var entry in request.Hours)
        {
            var existing = existingHours.FirstOrDefault(h => h.DayOfWeek == entry.DayOfWeek);

            if (existing is not null)
            {
                existing.OpenTime = entry.OpenTime;
                existing.CloseTime = entry.CloseTime;
                existing.IsClosed = entry.IsClosed;
            }
            else
            {
                var newHours = new RestaurantOperatingHours
                {
                    Id = Guid.CreateVersion7(),
                    RestaurantId = request.RestaurantId,
                    DayOfWeek = entry.DayOfWeek,
                    OpenTime = entry.OpenTime,
                    CloseTime = entry.CloseTime,
                    IsClosed = entry.IsClosed
                };

                db.RestaurantOperatingHours.Add(newHours);
            }
        }

        await db.SaveChangesAsync(ct);

        var allHours = await db.RestaurantOperatingHours
            .Where(h => h.RestaurantId == request.RestaurantId)
            .OrderBy(h => h.DayOfWeek)
            .ToListAsync(ct);

        var dtos = allHours
            .Select(h => new OperatingHoursDto(h.Id, h.DayOfWeek, h.OpenTime, h.CloseTime, h.IsClosed))
            .ToList();

        return Result<List<OperatingHoursDto>>.Success(dtos);
    }
}
