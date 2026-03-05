using FluentValidation;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Application.Features.Reviews.Commands.ResolveReviewReport;

public sealed class ResolveReviewReportCommandValidator : AbstractValidator<ResolveReviewReportCommand>
{
    public ResolveReviewReportCommandValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty();
        RuleFor(x => x.AdminId).NotEmpty();
        RuleFor(x => x.Status)
            .Must(s => s is ReviewReportStatus.ActionTaken or ReviewReportStatus.Dismissed)
            .WithMessage("Status must be ActionTaken or Dismissed.");
    }
}
