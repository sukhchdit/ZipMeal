using FluentValidation;

namespace SwiggyClone.Application.Features.PlatformConfig.Commands;

public sealed class UpdatePlatformConfigCommandValidator : AbstractValidator<UpdatePlatformConfigCommand>
{
    public UpdatePlatformConfigCommandValidator()
    {
        RuleFor(x => x.DeliveryFeePaise).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PackagingChargePaise).GreaterThanOrEqualTo(0);
        RuleFor(x => x.TaxRatePercent).InclusiveBetween(0, 100);
        RuleFor(x => x.FreeDeliveryThresholdPaise).GreaterThanOrEqualTo(0)
            .When(x => x.FreeDeliveryThresholdPaise.HasValue);
    }
}
