using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Promotions.Commands;

public sealed record UpdatePromotionCommand(
    Guid PromotionId,
    Guid OwnerId,
    string Title,
    string? Description,
    string? ImageUrl,
    DiscountType DiscountType,
    int DiscountValue,
    int? MaxDiscount,
    int? MinOrderAmount,
    DateTimeOffset ValidFrom,
    DateTimeOffset ValidUntil,
    int DisplayOrder,
    TimeOnly? RecurringStartTime,
    TimeOnly? RecurringEndTime,
    short[]? RecurringDaysOfWeek,
    int? ComboPrice,
    List<CreatePromotionMenuItemInput> MenuItems) : IRequest<Result>;

internal sealed class UpdatePromotionCommandHandler(IAppDbContext db)
    : IRequestHandler<UpdatePromotionCommand, Result>
{
    public async Task<Result> Handle(UpdatePromotionCommand request, CancellationToken ct)
    {
        var restaurant = await db.Restaurants
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.OwnerId == request.OwnerId, ct);

        if (restaurant is null)
            return Result.Failure("RESTAURANT_NOT_FOUND", "Restaurant not found.");

        var promotion = await db.RestaurantPromotions
            .Include(p => p.PromotionMenuItems)
            .FirstOrDefaultAsync(p => p.Id == request.PromotionId
                && p.RestaurantId == restaurant.Id, ct);

        if (promotion is null)
            return Result.Failure("PROMOTION_NOT_FOUND", "Promotion not found.");

        var menuItemIds = request.MenuItems.Select(m => m.MenuItemId).ToList();
        var validMenuItems = await db.MenuItems
            .AsNoTracking()
            .Where(mi => menuItemIds.Contains(mi.Id) && mi.RestaurantId == restaurant.Id)
            .Select(mi => mi.Id)
            .ToListAsync(ct);

        if (validMenuItems.Count != menuItemIds.Count)
            return Result.Failure("MENU_ITEM_NOT_IN_RESTAURANT",
                "One or more menu items do not belong to this restaurant.");

        promotion.Update(
            request.Title,
            request.Description,
            request.ImageUrl,
            request.DiscountType,
            request.DiscountValue,
            request.MaxDiscount,
            request.MinOrderAmount,
            request.ValidFrom,
            request.ValidUntil,
            request.DisplayOrder,
            request.RecurringStartTime,
            request.RecurringEndTime,
            request.RecurringDaysOfWeek,
            request.ComboPrice);

        // Sync menu items: remove old, add new
        foreach (var existing in promotion.PromotionMenuItems.ToList())
        {
            db.PromotionMenuItems.Remove(existing);
        }

        foreach (var item in request.MenuItems)
        {
            db.PromotionMenuItems.Add(new PromotionMenuItem
            {
                PromotionId = promotion.Id,
                MenuItemId = item.MenuItemId,
                Quantity = item.Quantity,
            });
        }

        await db.SaveChangesAsync(ct);

        return Result.Success();
    }
}

public sealed class UpdatePromotionCommandValidator : AbstractValidator<UpdatePromotionCommand>
{
    public UpdatePromotionCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ImageUrl).MaximumLength(500);
        RuleFor(x => x.DiscountValue).GreaterThan(0);
        RuleFor(x => x.DiscountValue).LessThanOrEqualTo(100)
            .When(x => x.DiscountType == DiscountType.Percentage)
            .WithMessage("Percentage discount must be between 1 and 100.");
        RuleFor(x => x.ValidFrom).LessThan(x => x.ValidUntil)
            .WithMessage("ValidFrom must be before ValidUntil.");
        RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MenuItems).NotEmpty()
            .WithMessage("At least one menu item is required.");
    }
}
