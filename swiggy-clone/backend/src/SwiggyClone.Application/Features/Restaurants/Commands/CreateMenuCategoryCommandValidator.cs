using FluentValidation;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

public sealed class CreateMenuCategoryCommandValidator : AbstractValidator<CreateMenuCategoryCommand>
{
    public CreateMenuCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100).WithMessage("Category name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
            .When(x => x.Description is not null);
    }
}
