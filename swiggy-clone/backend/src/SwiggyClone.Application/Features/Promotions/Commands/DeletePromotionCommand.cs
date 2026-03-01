using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Promotions.Commands;

public sealed record DeletePromotionCommand(Guid PromotionId, Guid OwnerId) : IRequest<Result>;

internal sealed class DeletePromotionCommandHandler(IAppDbContext db)
    : IRequestHandler<DeletePromotionCommand, Result>
{
    public async Task<Result> Handle(DeletePromotionCommand request, CancellationToken ct)
    {
        var restaurant = await db.Restaurants
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.OwnerId == request.OwnerId, ct);

        if (restaurant is null)
            return Result.Failure("RESTAURANT_NOT_FOUND", "Restaurant not found.");

        var promotion = await db.RestaurantPromotions
            .FirstOrDefaultAsync(p => p.Id == request.PromotionId
                && p.RestaurantId == restaurant.Id, ct);

        if (promotion is null)
            return Result.Failure("PROMOTION_NOT_FOUND", "Promotion not found.");

        promotion.SoftDelete();
        await db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
