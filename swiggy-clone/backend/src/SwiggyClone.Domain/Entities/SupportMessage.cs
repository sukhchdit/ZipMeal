using SwiggyClone.Domain.Common;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Domain.Entities;

public sealed class SupportMessage : BaseEntity
{
    public Guid TicketId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public SupportMessageType MessageType { get; set; }
    public bool IsRead { get; set; }
    public DateTimeOffset? ReadAt { get; set; }

    // Navigation Properties
    public SupportTicket Ticket { get; set; } = null!;
    public User Sender { get; set; } = null!;
}
