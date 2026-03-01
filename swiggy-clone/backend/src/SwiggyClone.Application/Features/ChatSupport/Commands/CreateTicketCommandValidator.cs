using FluentValidation;

namespace SwiggyClone.Application.Features.ChatSupport.Commands;

public sealed class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
{
    public CreateTicketCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Category).InclusiveBetween(0, 5)
            .WithMessage("Category must be between 0 (General) and 5 (Other).");
        RuleFor(x => x.InitialMessage).MaximumLength(4000)
            .When(x => x.InitialMessage is not null);
    }
}
