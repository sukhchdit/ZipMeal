using FluentValidation;

namespace SwiggyClone.Application.Features.DineIn.Commands;

public sealed class EndSessionCommandValidator : AbstractValidator<EndSessionCommand>
{
    public EndSessionCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.SessionId).NotEmpty();
    }
}
