using FluentValidation;

namespace SwiggyClone.Application.Features.Banners.Commands;

public sealed class ToggleBannerCommandValidator : AbstractValidator<ToggleBannerCommand>
{
    public ToggleBannerCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
