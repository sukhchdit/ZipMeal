using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class DeliveryPartnerLocationConfiguration : IEntityTypeConfiguration<DeliveryPartnerLocation>
{
    public void Configure(EntityTypeBuilder<DeliveryPartnerLocation> builder)
    {
        builder.ToTable("delivery_partner_locations");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.Latitude)
            .IsRequired();

        builder.Property(e => e.Longitude)
            .IsRequired();

        builder.Property(e => e.IsOnline)
            .HasDefaultValue(false);

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        // ───────────────────────── Relationships ─────────────────────────

        builder.HasOne(e => e.Partner)
            .WithMany()
            .HasForeignKey(e => e.PartnerId)
            .OnDelete(DeleteBehavior.Cascade);

        // ───────────────────────── Indexes ─────────────────────────

        builder.HasIndex(e => e.PartnerId)
            .HasDatabaseName("idx_delivery_partner_locations_partner");
    }
}
