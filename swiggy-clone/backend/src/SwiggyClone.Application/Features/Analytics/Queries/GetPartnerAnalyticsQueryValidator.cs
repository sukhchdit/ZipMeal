using FluentValidation;

namespace SwiggyClone.Application.Features.Analytics.Queries;

public sealed class GetPartnerAnalyticsQueryValidator
    : AbstractValidator<GetPartnerAnalyticsQuery>
{
    private static readonly string[] ValidPeriods = ["daily", "weekly", "monthly"];

    public GetPartnerAnalyticsQueryValidator()
    {
        RuleFor(x => x.Period)
            .Must(p => ValidPeriods.Contains(p))
            .WithMessage("Period must be 'daily', 'weekly', or 'monthly'.");

        RuleFor(x => x.Days)
            .InclusiveBetween(7, 365);
    }
}
