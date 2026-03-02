using MediatR;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Recommendations.Commands.TrackInteraction;

internal sealed class TrackInteractionCommandHandler(IAppDbContext db)
    : IRequestHandler<TrackInteractionCommand, Result>
{
    public async Task<Result> Handle(TrackInteractionCommand request, CancellationToken ct)
    {
        var interaction = new UserInteraction
        {
            Id = Guid.CreateVersion7(),
            UserId = request.UserId,
            EntityType = request.EntityType,
            EntityId = request.EntityId,
            InteractionType = request.InteractionType,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        db.UserInteractions.Add(interaction);
        await db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
