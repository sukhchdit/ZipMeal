using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class UserVariantAssignmentConfiguration : IEntityTypeConfiguration<UserVariantAssignment>
{
    public void Configure(EntityTypeBuilder<UserVariantAssignment> builder)
    {
        builder.ToTable("user_variant_assignments");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.AssignedAt)
            .IsRequired();

        // Relationships
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.Experiment)
            .WithMany(exp => exp.Assignments)
            .HasForeignKey(e => e.ExperimentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Variant)
            .WithMany()
            .HasForeignKey(e => e.VariantId)
            .OnDelete(DeleteBehavior.NoAction);

        // Indexes
        builder.HasIndex(e => new { e.UserId, e.ExperimentId })
            .IsUnique()
            .HasDatabaseName("idx_user_variant_assignments_user_experiment");
    }
}
