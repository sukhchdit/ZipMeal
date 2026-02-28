using FluentValidation;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

public sealed class UpdateMenuItemVariantCommandValidator : AbstractValidator<UpdateMenuItemVariantCommand>
{
    public UpdateMenuItemVariantCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Variant name is required.")
            .MaximumLength(100).WithMessage("Variant name must not exceed 100 characters.");
    }
}
