using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Domain.Entities;

public sealed class UserInteraction
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public InteractionEntityType EntityType { get; set; }
    public Guid EntityId { get; set; }
    public InteractionType InteractionType { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    // Navigation
    public User User { get; set; } = null!;
}
