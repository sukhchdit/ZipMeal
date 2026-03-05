using FluentValidation;

namespace SwiggyClone.Application.Features.Loyalty.Commands.EarnPoints;

public sealed class EarnPointsCommandValidator : AbstractValidator<EarnPointsCommand>
{
    public EarnPointsCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.OrderId).NotEmpty();
    }
}
