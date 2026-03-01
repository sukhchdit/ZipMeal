using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Promotions.Dtos;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Promotions.Queries;

public sealed record GetPromotionByIdQuery(
    Guid PromotionId,
    Guid OwnerId) : IRequest<Result<PromotionDto>>;

internal sealed class GetPromotionByIdQueryHandler(IAppDbContext db)
    : IRequestHandler<GetPromotionByIdQuery, Result<PromotionDto>>
{
    public async Task<Result<PromotionDto>> Handle(
        GetPromotionByIdQuery request, CancellationToken ct)
    {
        var restaurant = await db.Restaurants
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.OwnerId == request.OwnerId, ct);

        if (restaurant is null)
            return Result<PromotionDto>.Failure("RESTAURANT_NOT_FOUND", "Restaurant not found.");

        var promotion = await db.RestaurantPromotions
            .AsNoTracking()
            .Include(p => p.PromotionMenuItems)
                .ThenInclude(pmi => pmi.MenuItem)
            .FirstOrDefaultAsync(p => p.Id == request.PromotionId
                && p.RestaurantId == restaurant.Id, ct);

        if (promotion is null)
            return Result<PromotionDto>.Failure("PROMOTION_NOT_FOUND", "Promotion not found.");

        var dto = new PromotionDto(
            promotion.Id,
            promotion.RestaurantId,
            promotion.Title,
            promotion.Description,
            promotion.ImageUrl,
            (short)promotion.PromotionType,
            (short)promotion.DiscountType,
            promotion.DiscountValue,
            promotion.MaxDiscount,
            promotion.MinOrderAmount,
            promotion.ValidFrom,
            promotion.ValidUntil,
            promotion.IsActive,
            promotion.DisplayOrder,
            promotion.RecurringStartTime?.ToString("HH:mm", System.Globalization.CultureInfo.InvariantCulture),
            promotion.RecurringEndTime?.ToString("HH:mm", System.Globalization.CultureInfo.InvariantCulture),
            promotion.RecurringDaysOfWeek,
            promotion.ComboPrice,
            promotion.PromotionMenuItems.Select(pmi => new PromotionMenuItemDto(
                pmi.MenuItemId,
                pmi.MenuItem.Name,
                pmi.MenuItem.Price,
                pmi.MenuItem.DiscountedPrice,
                pmi.Quantity)).ToList(),
            promotion.CreatedAt,
            promotion.UpdatedAt);

        return Result<PromotionDto>.Success(dto);
    }
}
