using FluentValidation;

namespace SwiggyClone.Application.Features.Disputes.Commands.AddDisputeMessage;

public sealed class AddDisputeMessageCommandValidator : AbstractValidator<AddDisputeMessageCommand>
{
    public AddDisputeMessageCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.DisputeId).NotEmpty();
        RuleFor(x => x.Content).NotEmpty().MaximumLength(4000);
    }
}
