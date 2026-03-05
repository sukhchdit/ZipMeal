using SwiggyClone.Domain.Common;

namespace SwiggyClone.Domain.Entities;

public sealed class ExperimentVariant : BaseEntity
{
    public Guid ExperimentId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int AllocationPercent { get; set; }
    public string? ConfigJson { get; set; }
    public bool IsControl { get; set; }

    // Navigation
    public Experiment Experiment { get; set; } = null!;
}
