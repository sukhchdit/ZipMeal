using FluentValidation;

namespace SwiggyClone.Application.Features.Discovery.Queries;

public sealed class SearchRestaurantsQueryValidator : AbstractValidator<SearchRestaurantsQuery>
{
    public SearchRestaurantsQueryValidator()
    {
        RuleFor(x => x.Term).NotEmpty().MinimumLength(2).MaximumLength(100);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
    }
}
