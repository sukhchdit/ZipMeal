using FluentValidation;

namespace SwiggyClone.Application.Features.Discovery.Queries;

public sealed class GetSearchSuggestionsQueryValidator : AbstractValidator<GetSearchSuggestionsQuery>
{
    public GetSearchSuggestionsQueryValidator()
    {
        RuleFor(x => x.Prefix).NotEmpty().MinimumLength(1).MaximumLength(100);
        RuleFor(x => x.Limit).InclusiveBetween(1, 20);
    }
}
