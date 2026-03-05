using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class ExperimentConfiguration : IEntityTypeConfiguration<Experiment>
{
    public void Configure(EntityTypeBuilder<Experiment> builder)
    {
        builder.ToTable("experiments");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Key)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasDefaultValue(ExperimentStatus.Draft)
            .HasConversion<short>();

        builder.Property(e => e.TargetAudience)
            .HasMaxLength(500);

        builder.Property(e => e.GoalDescription)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(e => e.CreatedByUser)
            .WithMany()
            .HasForeignKey(e => e.CreatedByUserId)
            .OnDelete(DeleteBehavior.NoAction);

        // Indexes
        builder.HasIndex(e => e.Key)
            .IsUnique()
            .HasDatabaseName("idx_experiments_key");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("idx_experiments_status")
            .HasFilter("\"is_deleted\" = false");
    }
}
