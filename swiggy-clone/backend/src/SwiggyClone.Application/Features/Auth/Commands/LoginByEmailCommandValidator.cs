using FluentValidation;

namespace SwiggyClone.Application.Features.Auth.Commands;

public sealed class LoginByEmailCommandValidator : AbstractValidator<LoginByEmailCommand>
{
    public LoginByEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
