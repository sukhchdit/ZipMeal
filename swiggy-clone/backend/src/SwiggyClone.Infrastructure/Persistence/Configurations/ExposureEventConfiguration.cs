using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class ExposureEventConfiguration : IEntityTypeConfiguration<ExposureEvent>
{
    public void Configure(EntityTypeBuilder<ExposureEvent> builder)
    {
        builder.ToTable("exposure_events");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Context)
            .HasMaxLength(500);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        // Relationships
        builder.HasOne(e => e.Assignment)
            .WithMany(a => a.Exposures)
            .HasForeignKey(e => e.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => e.AssignmentId)
            .HasDatabaseName("idx_exposure_events_assignment");
    }
}
