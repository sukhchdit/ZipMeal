using FluentValidation;

namespace SwiggyClone.Application.Features.DineIn.Commands;

public sealed class JoinSessionCommandValidator : AbstractValidator<JoinSessionCommand>
{
    public JoinSessionCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.SessionCode).NotEmpty().MaximumLength(10)
            .WithMessage("Session code is required.");
    }
}
