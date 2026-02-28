using FluentValidation;

namespace SwiggyClone.Application.Features.Deliveries.Commands;

public sealed class AssignDeliveryPartnerCommandValidator
    : AbstractValidator<AssignDeliveryPartnerCommand>
{
    public AssignDeliveryPartnerCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
    }
}
