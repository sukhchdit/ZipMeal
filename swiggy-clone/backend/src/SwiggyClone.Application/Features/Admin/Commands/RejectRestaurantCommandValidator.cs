using FluentValidation;

namespace SwiggyClone.Application.Features.Admin.Commands;

public sealed class RejectRestaurantCommandValidator : AbstractValidator<RejectRestaurantCommand>
{
    public RejectRestaurantCommandValidator()
    {
        RuleFor(x => x.RestaurantId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}
