using FluentValidation;

namespace SwiggyClone.Application.Features.DineIn.Commands;

public sealed class PlaceDineInOrderCommandValidator : AbstractValidator<PlaceDineInOrderCommand>
{
    public PlaceDineInOrderCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.SessionId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty().WithMessage("At least one item is required.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.MenuItemId).NotEmpty();
            item.RuleFor(i => i.Quantity).GreaterThan(0).LessThanOrEqualTo(20);
            item.RuleFor(i => i.SpecialInstructions).MaximumLength(500);
        });
        RuleFor(x => x.SpecialInstructions).MaximumLength(500);
    }
}
