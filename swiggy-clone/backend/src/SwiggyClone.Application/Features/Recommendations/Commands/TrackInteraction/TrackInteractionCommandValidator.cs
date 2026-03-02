using FluentValidation;

namespace SwiggyClone.Application.Features.Recommendations.Commands.TrackInteraction;

public sealed class TrackInteractionCommandValidator : AbstractValidator<TrackInteractionCommand>
{
    public TrackInteractionCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.EntityId).NotEmpty();
        RuleFor(x => x.EntityType).IsInEnum();
        RuleFor(x => x.InteractionType).IsInEnum();
    }
}
