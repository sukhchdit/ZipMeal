using FluentValidation;

namespace SwiggyClone.Application.Features.Admin.Commands;

public sealed class ToggleUserActiveCommandValidator : AbstractValidator<ToggleUserActiveCommand>
{
    public ToggleUserActiveCommandValidator()
    {
        RuleFor(x => x.TargetUserId).NotEmpty();
    }
}
