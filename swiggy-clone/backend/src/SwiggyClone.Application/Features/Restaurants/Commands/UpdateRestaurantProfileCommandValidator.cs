using FluentValidation;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

public sealed class UpdateRestaurantProfileCommandValidator : AbstractValidator<UpdateRestaurantProfileCommand>
{
    public UpdateRestaurantProfileCommandValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(200).WithMessage("Restaurant name must not exceed 200 characters.")
            .When(x => x.Name is not null);

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[1-9]\d{6,14}$").WithMessage("Phone number must be in E.164 format.")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters.")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.City)
            .MaximumLength(100).WithMessage("City must not exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.City));

        RuleFor(x => x.State)
            .MaximumLength(100).WithMessage("State must not exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.State));

        RuleFor(x => x.PostalCode)
            .MaximumLength(10).WithMessage("Postal code must not exceed 10 characters.")
            .When(x => !string.IsNullOrEmpty(x.PostalCode));

        RuleFor(x => x.CuisineIds)
            .Must(ids => ids!.Count <= 10).WithMessage("A maximum of 10 cuisine types can be selected.")
            .When(x => x.CuisineIds is not null);
    }
}
