using SwiggyClone.Domain.Common;

namespace SwiggyClone.Domain.Entities;

public sealed class DisputeMessage : BaseEntity
{
    public Guid DisputeId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsSystemMessage { get; set; }
    public bool IsRead { get; set; }
    public DateTimeOffset? ReadAt { get; set; }

    // Navigation
    public Dispute Dispute { get; set; } = null!;
    public User Sender { get; set; } = null!;
}
