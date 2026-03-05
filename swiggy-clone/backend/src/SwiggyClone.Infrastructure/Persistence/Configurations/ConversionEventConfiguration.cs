using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class ConversionEventConfiguration : IEntityTypeConfiguration<ConversionEvent>
{
    public void Configure(EntityTypeBuilder<ConversionEvent> builder)
    {
        builder.ToTable("conversion_events");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.GoalKey)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Value)
            .HasPrecision(18, 4);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        // Relationships
        builder.HasOne(e => e.Assignment)
            .WithMany(a => a.Conversions)
            .HasForeignKey(e => e.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => new { e.AssignmentId, e.GoalKey })
            .HasDatabaseName("idx_conversion_events_assignment_goal");
    }
}
