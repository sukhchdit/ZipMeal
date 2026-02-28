using FluentValidation;

namespace SwiggyClone.Application.Features.Deliveries.Commands;

public sealed class UpdateDeliveryStatusCommandValidator
    : AbstractValidator<UpdateDeliveryStatusCommand>
{
    public UpdateDeliveryStatusCommandValidator()
    {
        RuleFor(x => x.PartnerId).NotEmpty();
        RuleFor(x => x.AssignmentId).NotEmpty();
        RuleFor(x => x.NewStatus).InclusiveBetween(2, 4)
            .WithMessage("NewStatus must be 2 (PickedUp), 3 (EnRoute), or 4 (Delivered).");
    }
}
