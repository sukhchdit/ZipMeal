using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class ExperimentVariantConfiguration : IEntityTypeConfiguration<ExperimentVariant>
{
    public void Configure(EntityTypeBuilder<ExperimentVariant> builder)
    {
        builder.ToTable("experiment_variants");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Key)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.AllocationPercent)
            .IsRequired();

        builder.Property(e => e.ConfigJson)
            .HasColumnType("jsonb");

        builder.Property(e => e.IsControl)
            .IsRequired()
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(e => e.Experiment)
            .WithMany(exp => exp.Variants)
            .HasForeignKey(e => e.ExperimentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => new { e.ExperimentId, e.Key })
            .IsUnique()
            .HasDatabaseName("idx_experiment_variants_experiment_key");
    }
}
