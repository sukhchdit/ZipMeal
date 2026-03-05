using FluentValidation;

namespace SwiggyClone.Application.Features.Disputes.Commands.CreateDispute;

public sealed class CreateDisputeCommandValidator : AbstractValidator<CreateDisputeCommand>
{
    public CreateDisputeCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.IssueType).InclusiveBetween(0, 7)
            .WithMessage("Issue type must be between 0 and 7.");
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
    }
}
