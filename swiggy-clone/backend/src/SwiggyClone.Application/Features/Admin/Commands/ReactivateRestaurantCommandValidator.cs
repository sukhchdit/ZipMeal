using FluentValidation;

namespace SwiggyClone.Application.Features.Admin.Commands;

public sealed class ReactivateRestaurantCommandValidator : AbstractValidator<ReactivateRestaurantCommand>
{
    public ReactivateRestaurantCommandValidator()
    {
        RuleFor(x => x.RestaurantId).NotEmpty();
    }
}
