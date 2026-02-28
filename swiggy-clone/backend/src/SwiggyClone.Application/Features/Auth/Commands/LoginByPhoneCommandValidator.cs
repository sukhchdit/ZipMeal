using FluentValidation;

namespace SwiggyClone.Application.Features.Auth.Commands;

public sealed class LoginByPhoneCommandValidator : AbstractValidator<LoginByPhoneCommand>
{
    public LoginByPhoneCommandValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[1-9]\d{6,14}$").WithMessage("Phone number must be in E.164 format.");

        RuleFor(x => x.Otp)
            .NotEmpty().WithMessage("OTP is required.")
            .Length(6).WithMessage("OTP must be exactly 6 digits.");
    }
}
