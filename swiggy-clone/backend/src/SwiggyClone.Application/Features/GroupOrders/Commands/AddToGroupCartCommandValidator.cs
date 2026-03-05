using FluentValidation;

namespace SwiggyClone.Application.Features.GroupOrders.Commands;

public sealed class AddToGroupCartCommandValidator : AbstractValidator<AddToGroupCartCommand>
{
    public AddToGroupCartCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.GroupOrderId).NotEmpty();
        RuleFor(x => x.MenuItemId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0).LessThanOrEqualTo(20);
    }
}
