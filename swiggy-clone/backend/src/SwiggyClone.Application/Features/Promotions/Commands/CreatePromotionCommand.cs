using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Promotions.Commands;

public sealed record CreatePromotionCommand(
    Guid OwnerId,
    string Title,
    string? Description,
    string? ImageUrl,
    PromotionType PromotionType,
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
    List<CreatePromotionMenuItemInput> MenuItems) : IRequest<Result<Guid>>;

public sealed record CreatePromotionMenuItemInput(Guid MenuItemId, int Quantity = 1);

internal sealed class CreatePromotionCommandHandler(IAppDbContext db)
    : IRequestHandler<CreatePromotionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreatePromotionCommand request, CancellationToken ct)
    {
        var restaurant = await db.Restaurants
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.OwnerId == request.OwnerId, ct);

        if (restaurant is null)
            return Result<Guid>.Failure("RESTAURANT_NOT_FOUND", "Restaurant not found.");

        var menuItemIds = request.MenuItems.Select(m => m.MenuItemId).ToList();
        var validMenuItems = await db.MenuItems
            .AsNoTracking()
            .Where(mi => menuItemIds.Contains(mi.Id) && mi.RestaurantId == restaurant.Id)
            .Select(mi => mi.Id)
            .ToListAsync(ct);

        if (validMenuItems.Count != menuItemIds.Count)
            return Result<Guid>.Failure("MENU_ITEM_NOT_IN_RESTAURANT",
                "One or more menu items do not belong to this restaurant.");

        var promotion = RestaurantPromotion.Create(
            restaurant.Id,
            request.Title,
            request.Description,
            request.ImageUrl,
            request.PromotionType,
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

        db.RestaurantPromotions.Add(promotion);

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

        return Result<Guid>.Success(promotion.Id);
    }
}

public sealed class CreatePromotionCommandValidator : AbstractValidator<CreatePromotionCommand>
{
    public CreatePromotionCommandValidator()
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

        RuleFor(x => x.RecurringStartTime).NotNull()
            .When(x => x.PromotionType == PromotionType.HappyHour)
            .WithMessage("Recurring start time is required for happy hours.");
        RuleFor(x => x.RecurringEndTime).NotNull()
            .When(x => x.PromotionType == PromotionType.HappyHour)
            .WithMessage("Recurring end time is required for happy hours.");
        RuleFor(x => x.RecurringDaysOfWeek).NotEmpty()
            .When(x => x.PromotionType == PromotionType.HappyHour)
            .WithMessage("Recurring days are required for happy hours.");

        RuleFor(x => x.ComboPrice).NotNull().GreaterThan(0)
            .When(x => x.PromotionType == PromotionType.ComboOffer)
            .WithMessage("Combo price is required for combo offers.");
    }
}
