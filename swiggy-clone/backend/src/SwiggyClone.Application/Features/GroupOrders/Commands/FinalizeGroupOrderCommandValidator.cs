using FluentValidation;

namespace SwiggyClone.Application.Features.GroupOrders.Commands;

public sealed class FinalizeGroupOrderCommandValidator : AbstractValidator<FinalizeGroupOrderCommand>
{
    public FinalizeGroupOrderCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.GroupOrderId).NotEmpty();
        RuleFor(x => x.DeliveryAddressId).NotEmpty();
        RuleFor(x => x.PaymentMethod).InclusiveBetween(1, 5);
    }
}
