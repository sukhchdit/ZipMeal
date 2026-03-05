namespace SwiggyClone.Domain.Entities;

public sealed class ConversionEvent
{
    public Guid Id { get; set; }
    public Guid AssignmentId { get; set; }
    public string GoalKey { get; set; } = string.Empty;
    public decimal? Value { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    // Navigation
    public UserVariantAssignment Assignment { get; set; } = null!;
}
