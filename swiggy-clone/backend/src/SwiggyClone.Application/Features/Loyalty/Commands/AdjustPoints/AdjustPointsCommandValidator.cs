using FluentValidation;

namespace SwiggyClone.Application.Features.Loyalty.Commands.AdjustPoints;

public sealed class AdjustPointsCommandValidator : AbstractValidator<AdjustPointsCommand>
{
    public AdjustPointsCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Points).NotEqual(0);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
    }
}
