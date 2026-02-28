using FluentValidation;

namespace SwiggyClone.Application.Features.DineIn.Commands;

public sealed class LeaveSessionCommandValidator : AbstractValidator<LeaveSessionCommand>
{
    public LeaveSessionCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.SessionId).NotEmpty();
    }
}
