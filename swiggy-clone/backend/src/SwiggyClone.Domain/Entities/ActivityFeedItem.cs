using SwiggyClone.Domain.Common;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Domain.Entities;

public sealed class ActivityFeedItem : BaseEntity
{
    public Guid UserId { get; set; }
    public ActivityType ActivityType { get; set; }
    public Guid? TargetEntityId { get; set; }
    public string? Metadata { get; set; }

    public User User { get; set; } = null!;
}
