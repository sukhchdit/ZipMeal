using SwiggyClone.Domain.Common;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Domain.Entities;

public sealed class CannedResponse : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public SupportTicketCategory Category { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
