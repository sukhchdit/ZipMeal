namespace SwiggyClone.Domain.Entities;

public sealed class UserVariantAssignment
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ExperimentId { get; set; }
    public Guid VariantId { get; set; }
    public DateTimeOffset AssignedAt { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public Experiment Experiment { get; set; } = null!;
    public ExperimentVariant Variant { get; set; } = null!;
    public ICollection<ExposureEvent> Exposures { get; set; } = [];
    public ICollection<ConversionEvent> Conversions { get; set; } = [];
}
