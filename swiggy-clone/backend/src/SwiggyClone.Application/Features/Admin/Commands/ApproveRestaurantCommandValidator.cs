using FluentValidation;

namespace SwiggyClone.Application.Features.Admin.Commands;

public sealed class ApproveRestaurantCommandValidator : AbstractValidator<ApproveRestaurantCommand>
{
    public ApproveRestaurantCommandValidator()
    {
        RuleFor(x => x.RestaurantId).NotEmpty();
    }
}
