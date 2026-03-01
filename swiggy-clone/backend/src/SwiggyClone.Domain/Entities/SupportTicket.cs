using SwiggyClone.Domain.Common;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Domain.Entities;

public sealed class SupportTicket : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid? AssignedAgentId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public SupportTicketCategory Category { get; set; }
    public SupportTicketStatus Status { get; set; }
    public Guid? OrderId { get; set; }
    public DateTimeOffset? ClosedAt { get; set; }

    // Navigation Properties
    public User User { get; set; } = null!;
    public User? AssignedAgent { get; set; }
    public Order? Order { get; set; }
    public ICollection<SupportMessage> Messages { get; set; } = [];
}
