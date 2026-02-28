using FluentValidation;

namespace SwiggyClone.Application.Features.Deliveries.Commands;

public sealed class AcceptDeliveryCommandValidator : AbstractValidator<AcceptDeliveryCommand>
{
    public AcceptDeliveryCommandValidator()
    {
        RuleFor(x => x.PartnerId).NotEmpty();
        RuleFor(x => x.AssignmentId).NotEmpty();
    }
}
