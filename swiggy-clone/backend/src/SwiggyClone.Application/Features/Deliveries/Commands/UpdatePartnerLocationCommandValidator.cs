using FluentValidation;

namespace SwiggyClone.Application.Features.Deliveries.Commands;

public sealed class UpdatePartnerLocationCommandValidator
    : AbstractValidator<UpdatePartnerLocationCommand>
{
    public UpdatePartnerLocationCommandValidator()
    {
        RuleFor(x => x.PartnerId).NotEmpty();
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
    }
}
