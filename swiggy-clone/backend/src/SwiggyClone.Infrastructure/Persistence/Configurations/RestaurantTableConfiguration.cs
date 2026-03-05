using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class RestaurantTableConfiguration : IEntityTypeConfiguration<RestaurantTable>
{
    public void Configure(EntityTypeBuilder<RestaurantTable> builder)
    {
        builder.ToTable("restaurant_tables");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.TableNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.Capacity)
            .HasDefaultValue(4);

        builder.Property(e => e.FloorSection)
            .HasMaxLength(50);

        builder.Property(e => e.QrCodeData)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Status)
            .HasDefaultValue(TableStatus.Available)
            .HasConversion<short>();

        builder.Property(e => e.IsActive)
            .HasDefaultValue(true);

        // ───────────────────────── Relationships ─────────────────────────

        builder.HasOne(e => e.Restaurant)
            .WithMany(r => r.Tables)
            .HasForeignKey(e => e.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        // ───────────────────────── Indexes ─────────────────────────

        builder.HasIndex(e => e.QrCodeData)
            .IsUnique()
            .HasDatabaseName("idx_restaurant_tables_qr");

        builder.HasIndex(e => new { e.RestaurantId, e.TableNumber })
            .IsUnique();

        builder.HasIndex(e => e.RestaurantId)
            .HasDatabaseName("idx_restaurant_tables_restaurant")
            .HasFilter("\"is_active\" = true");
    }
}
