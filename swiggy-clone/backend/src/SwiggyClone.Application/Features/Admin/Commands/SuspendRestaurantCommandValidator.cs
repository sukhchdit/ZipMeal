using FluentValidation;

namespace SwiggyClone.Application.Features.Admin.Commands;

public sealed class SuspendRestaurantCommandValidator : AbstractValidator<SuspendRestaurantCommand>
{
    public SuspendRestaurantCommandValidator()
    {
        RuleFor(x => x.RestaurantId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}
