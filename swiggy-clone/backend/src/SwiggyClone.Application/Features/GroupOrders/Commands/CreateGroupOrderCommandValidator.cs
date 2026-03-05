using FluentValidation;

namespace SwiggyClone.Application.Features.GroupOrders.Commands;

public sealed class CreateGroupOrderCommandValidator : AbstractValidator<CreateGroupOrderCommand>
{
    public CreateGroupOrderCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.RestaurantId).NotEmpty();
        RuleFor(x => x.PaymentSplitType).IsInEnum();
    }
}
