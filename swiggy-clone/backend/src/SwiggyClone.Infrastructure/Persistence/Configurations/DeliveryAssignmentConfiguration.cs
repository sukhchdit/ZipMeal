using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class DeliveryAssignmentConfiguration : IEntityTypeConfiguration<DeliveryAssignment>
{
    public void Configure(EntityTypeBuilder<DeliveryAssignment> builder)
    {
        builder.ToTable("delivery_assignments");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.Status)
            .HasDefaultValue(DeliveryStatus.Assigned)
            .HasConversion<short>();

        builder.Property(e => e.AssignedAt)
            .IsRequired();

        builder.Property(e => e.DistanceKm)
            .HasColumnType("decimal(6,2)");

        builder.Property(e => e.Earnings)
            .HasDefaultValue(0);

        // ───────────────────────── Relationships ─────────────────────────

        builder.HasOne(e => e.Order)
            .WithMany()
            .HasForeignKey(e => e.OrderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.Partner)
            .WithMany()
            .HasForeignKey(e => e.PartnerId)
            .OnDelete(DeleteBehavior.NoAction);

        // ───────────────────────── Indexes ─────────────────────────

        builder.HasIndex(e => e.OrderId)
            .HasDatabaseName("idx_delivery_assignments_order");

        builder.HasIndex(e => new { e.PartnerId, e.Status })
            .HasDatabaseName("idx_delivery_assignments_partner");
    }
}
