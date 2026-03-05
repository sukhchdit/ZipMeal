using FluentValidation;

namespace SwiggyClone.Application.Features.GroupOrders.Commands;

public sealed class LeaveGroupOrderCommandValidator : AbstractValidator<LeaveGroupOrderCommand>
{
    public LeaveGroupOrderCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.GroupOrderId).NotEmpty();
    }
}
