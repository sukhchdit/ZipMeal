using FluentValidation;

namespace SwiggyClone.Application.Features.Auth.Commands;

public sealed class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        When(x => x.FullName is not null, () =>
        {
            RuleFor(x => x.FullName)
                .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");
        });

        When(x => x.Email is not null, () =>
        {
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("A valid email is required.")
                .MaximumLength(255);
        });

        When(x => x.AvatarUrl is not null, () =>
        {
            RuleFor(x => x.AvatarUrl)
                .MaximumLength(500);
        });
    }
}
