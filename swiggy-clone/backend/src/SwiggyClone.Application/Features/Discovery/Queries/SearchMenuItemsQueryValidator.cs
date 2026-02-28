using FluentValidation;

namespace SwiggyClone.Application.Features.Discovery.Queries;

public sealed class SearchMenuItemsQueryValidator : AbstractValidator<SearchMenuItemsQuery>
{
    public SearchMenuItemsQueryValidator()
    {
        RuleFor(x => x.Term).NotEmpty().MinimumLength(2).MaximumLength(100);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
    }
}
