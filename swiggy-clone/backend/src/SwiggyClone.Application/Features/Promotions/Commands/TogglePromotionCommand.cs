using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Promotions.Commands;

public sealed record TogglePromotionCommand(
    Guid PromotionId,
    bool IsActive,
    Guid? OwnerId = null) : IRequest<Result>;

internal sealed class TogglePromotionCommandHandler(IAppDbContext db)
    : IRequestHandler<TogglePromotionCommand, Result>
{
    public async Task<Result> Handle(TogglePromotionCommand request, CancellationToken ct)
    {
        var query = db.RestaurantPromotions.AsQueryable();

        if (request.OwnerId.HasValue)
        {
            var restaurant = await db.Restaurants
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.OwnerId == request.OwnerId.Value, ct);

            if (restaurant is null)
                return Result.Failure("RESTAURANT_NOT_FOUND", "Restaurant not found.");

            query = query.Where(p => p.RestaurantId == restaurant.Id);
        }

        var promotion = await query.FirstOrDefaultAsync(p => p.Id == request.PromotionId, ct);

        if (promotion is null)
            return Result.Failure("PROMOTION_NOT_FOUND", "Promotion not found.");

        if (promotion.IsActive == request.IsActive)
            return Result.Success();

        promotion.ToggleActive();
        await db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
