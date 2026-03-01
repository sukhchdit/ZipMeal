using FluentValidation;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

public sealed class CreateMenuItemCommandValidator : AbstractValidator<CreateMenuItemCommand>
{
    public CreateMenuItemCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Menu item name is required.")
            .MaximumLength(200).WithMessage("Menu item name must not exceed 200 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(x => x.DiscountedPrice)
            .GreaterThan(0).WithMessage("Discounted price must be greater than 0.")
            .LessThan(x => x.Price).WithMessage("Discounted price must be less than the regular price.")
            .When(x => x.DiscountedPrice.HasValue);

        RuleFor(x => x.PreparationTimeMin)
            .GreaterThan(0).WithMessage("Preparation time must be greater than 0.");

        RuleFor(x => x.SpiceLevel)
            .InclusiveBetween((short)0, (short)4).WithMessage("Invalid spice level.");

        RuleForEach(x => x.Allergens)
            .InclusiveBetween((short)0, (short)13).WithMessage("Invalid allergen value.")
            .When(x => x.Allergens is not null);

        RuleForEach(x => x.DietaryTags)
            .InclusiveBetween((short)0, (short)9).WithMessage("Invalid dietary tag value.")
            .When(x => x.DietaryTags is not null);

        RuleFor(x => x.CalorieCount)
            .GreaterThan(0).WithMessage("Calorie count must be greater than 0.")
            .When(x => x.CalorieCount.HasValue);
    }
}
