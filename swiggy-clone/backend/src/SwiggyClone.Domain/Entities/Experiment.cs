using SwiggyClone.Domain.Common;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Domain.Entities;

public sealed class Experiment : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ExperimentStatus Status { get; set; }
    public string? TargetAudience { get; set; }
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public string? GoalDescription { get; set; }
    public Guid CreatedByUserId { get; set; }

    // Navigation
    public User CreatedByUser { get; set; } = null!;
    public ICollection<ExperimentVariant> Variants { get; set; } = [];
    public ICollection<UserVariantAssignment> Assignments { get; set; } = [];
}
