using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Domain.Entities;

public sealed class ReviewReport
{
    public Guid Id { get; set; }
    public Guid ReviewId { get; set; }
    public Guid UserId { get; set; }
    public ReviewReportReason Reason { get; set; }
    public string? Description { get; set; }
    public ReviewReportStatus Status { get; set; }
    public string? AdminNotes { get; set; }
    public Guid? ResolvedByAdminId { get; set; }
    public DateTimeOffset? ResolvedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    // Navigation
    public Review Review { get; set; } = null!;
    public User User { get; set; } = null!;
    public User? ResolvedByAdmin { get; set; }
}
