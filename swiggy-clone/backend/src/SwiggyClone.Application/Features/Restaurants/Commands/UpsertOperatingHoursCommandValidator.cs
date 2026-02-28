using FluentValidation;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

public sealed class UpsertOperatingHoursCommandValidator : AbstractValidator<UpsertOperatingHoursCommand>
{
    public UpsertOperatingHoursCommandValidator()
    {
        RuleFor(x => x.Hours)
            .NotNull().WithMessage("Operating hours are required.")
            .Must(h => h.Count == 7).WithMessage("Exactly 7 operating hour entries are required (one per day).");

        RuleForEach(x => x.Hours).ChildRules(entry =>
        {
            entry.RuleFor(e => e.DayOfWeek)
                .InclusiveBetween((short)0, (short)6).WithMessage("Day of week must be between 0 (Sunday) and 6 (Saturday).");

            entry.RuleFor(e => e.OpenTime)
                .NotNull().WithMessage("Open time is required when the restaurant is not closed.")
                .When(e => !e.IsClosed);

            entry.RuleFor(e => e.CloseTime)
                .NotNull().WithMessage("Close time is required when the restaurant is not closed.")
                .When(e => !e.IsClosed);
        });
    }
}
