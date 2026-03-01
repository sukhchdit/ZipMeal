using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Promotions.Dtos;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Promotions.Queries;

public sealed record GetActivePromotionsQuery(Guid RestaurantId) : IRequest<Result<List<PromotionDto>>>;

internal sealed class GetActivePromotionsQueryHandler(IAppDbContext db)
    : IRequestHandler<GetActivePromotionsQuery, Result<List<PromotionDto>>>
{
    public async Task<Result<List<PromotionDto>>> Handle(
        GetActivePromotionsQuery request, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;

        var promotions = await db.RestaurantPromotions
            .AsNoTracking()
            .Include(p => p.PromotionMenuItems)
                .ThenInclude(pmi => pmi.MenuItem)
            .Where(p => p.RestaurantId == request.RestaurantId
                && p.IsActive
                && p.ValidFrom <= now
                && p.ValidUntil >= now)
            .OrderBy(p => p.DisplayOrder)
            .ThenByDescending(p => p.CreatedAt)
            .ToListAsync(ct);

        // Post-filter HappyHour promotions for current time/day
        var currentTime = TimeOnly.FromDateTime(now.UtcDateTime);
        var currentDay = (short)now.DayOfWeek;

        var filteredPromotions = promotions.Where(p =>
        {
            if (p.PromotionType != Domain.Enums.PromotionType.HappyHour)
                return true;

            if (p.RecurringStartTime is null || p.RecurringEndTime is null || p.RecurringDaysOfWeek is null)
                return true;

            return p.RecurringDaysOfWeek.Contains(currentDay)
                && currentTime >= p.RecurringStartTime.Value
                && currentTime <= p.RecurringEndTime.Value;
        }).ToList();

        var result = filteredPromotions.Select(p => new PromotionDto(
            p.Id,
            p.RestaurantId,
            p.Title,
            p.Description,
            p.ImageUrl,
            (short)p.PromotionType,
            (short)p.DiscountType,
            p.DiscountValue,
            p.MaxDiscount,
            p.MinOrderAmount,
            p.ValidFrom,
            p.ValidUntil,
            p.IsActive,
            p.DisplayOrder,
            p.RecurringStartTime?.ToString("HH:mm", System.Globalization.CultureInfo.InvariantCulture),
            p.RecurringEndTime?.ToString("HH:mm", System.Globalization.CultureInfo.InvariantCulture),
            p.RecurringDaysOfWeek,
            p.ComboPrice,
            p.PromotionMenuItems.Select(pmi => new PromotionMenuItemDto(
                pmi.MenuItemId,
                pmi.MenuItem.Name,
                pmi.MenuItem.Price,
                pmi.MenuItem.DiscountedPrice,
                pmi.Quantity)).ToList(),
            p.CreatedAt,
            p.UpdatedAt)).ToList();

        return Result<List<PromotionDto>>.Success(result);
    }
}
