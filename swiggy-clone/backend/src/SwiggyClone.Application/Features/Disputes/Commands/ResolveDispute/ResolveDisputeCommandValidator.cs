using FluentValidation;

namespace SwiggyClone.Application.Features.Disputes.Commands.ResolveDispute;

public sealed class ResolveDisputeCommandValidator : AbstractValidator<ResolveDisputeCommand>
{
    public ResolveDisputeCommandValidator()
    {
        RuleFor(x => x.AgentId).NotEmpty();
        RuleFor(x => x.DisputeId).NotEmpty();
        RuleFor(x => x.ResolutionType).InclusiveBetween(0, 5)
            .WithMessage("Resolution type must be between 0 and 5.");
        RuleFor(x => x.ResolutionAmountPaise).GreaterThan(0)
            .When(x => x.ResolutionType is 0 or 1 or 2)
            .WithMessage("Resolution amount is required for refund/credit resolutions.");
        RuleFor(x => x.ResolutionNotes).MaximumLength(2000);
    }
}
