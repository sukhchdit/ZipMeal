using FluentValidation;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

public sealed class AddMenuItemAddonCommandValidator : AbstractValidator<AddMenuItemAddonCommand>
{
    public AddMenuItemAddonCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Addon name is required.")
            .MaximumLength(100).WithMessage("Addon name must not exceed 100 characters.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0.");

        RuleFor(x => x.MaxQuantity)
            .GreaterThan(0).WithMessage("Max quantity must be greater than 0.")
            .LessThanOrEqualTo(10).WithMessage("Max quantity must not exceed 10.");
    }
}
