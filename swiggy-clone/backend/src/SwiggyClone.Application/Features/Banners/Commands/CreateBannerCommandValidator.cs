using FluentValidation;

namespace SwiggyClone.Application.Features.Banners.Commands;

public sealed class CreateBannerCommandValidator : AbstractValidator<CreateBannerCommand>
{
    public CreateBannerCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ImageUrl).NotEmpty().MaximumLength(500);
        RuleFor(x => x.DeepLink).MaximumLength(500);
        RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ValidFrom).LessThan(x => x.ValidUntil)
            .WithMessage("ValidFrom must be before ValidUntil.");
    }
}
