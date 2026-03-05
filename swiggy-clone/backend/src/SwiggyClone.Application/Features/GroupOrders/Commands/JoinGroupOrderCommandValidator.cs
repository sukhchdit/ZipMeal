using FluentValidation;

namespace SwiggyClone.Application.Features.GroupOrders.Commands;

public sealed class JoinGroupOrderCommandValidator : AbstractValidator<JoinGroupOrderCommand>
{
    public JoinGroupOrderCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.InviteCode).NotEmpty().MaximumLength(6);
    }
}
