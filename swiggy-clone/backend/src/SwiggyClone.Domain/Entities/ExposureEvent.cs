namespace SwiggyClone.Domain.Entities;

public sealed class ExposureEvent
{
    public Guid Id { get; set; }
    public Guid AssignmentId { get; set; }
    public string? Context { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    // Navigation
    public UserVariantAssignment Assignment { get; set; } = null!;
}
