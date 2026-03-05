using FluentValidation;

namespace SwiggyClone.Application.Features.GroupOrders.Commands;

public sealed class SetParticipantReadyCommandValidator : AbstractValidator<SetParticipantReadyCommand>
{
    public SetParticipantReadyCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.GroupOrderId).NotEmpty();
    }
}
