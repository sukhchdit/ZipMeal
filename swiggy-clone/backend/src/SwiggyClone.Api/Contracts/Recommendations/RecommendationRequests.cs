using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Api.Contracts.Recommendations;

public sealed record TrackInteractionRequest(
    InteractionEntityType EntityType,
    Guid EntityId,
    InteractionType InteractionType);
