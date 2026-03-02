using MediatR;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Recommendations.Commands.TrackInteraction;

public sealed record TrackInteractionCommand(
    Guid UserId,
    InteractionEntityType EntityType,
    Guid EntityId,
    InteractionType InteractionType) : IRequest<Result>;
