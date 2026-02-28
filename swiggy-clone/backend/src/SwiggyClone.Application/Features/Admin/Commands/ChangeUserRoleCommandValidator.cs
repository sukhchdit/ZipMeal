using FluentValidation;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Application.Features.Admin.Commands;

public sealed class ChangeUserRoleCommandValidator : AbstractValidator<ChangeUserRoleCommand>
{
    public ChangeUserRoleCommandValidator()
    {
        RuleFor(x => x.TargetUserId).NotEmpty();
        RuleFor(x => x.NewRole).IsInEnum()
            .Must(r => r >= UserRole.Customer && r <= UserRole.Admin)
            .WithMessage("Role must be between Customer (1) and Admin (4).");
    }
}
