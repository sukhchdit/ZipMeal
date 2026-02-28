using FluentValidation;

namespace SwiggyClone.Application.Features.DineIn.Commands;

public sealed class UpdateDineInOrderStatusCommandValidator
    : AbstractValidator<UpdateDineInOrderStatusCommand>
{
    public UpdateDineInOrderStatusCommandValidator()
    {
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.NewStatus).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}
