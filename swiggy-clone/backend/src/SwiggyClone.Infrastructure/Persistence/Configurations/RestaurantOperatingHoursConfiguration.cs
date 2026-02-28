using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class RestaurantOperatingHoursConfiguration : IEntityTypeConfiguration<RestaurantOperatingHours>
{
    public void Configure(EntityTypeBuilder<RestaurantOperatingHours> builder)
    {
        builder.ToTable("restaurant_operating_hours");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.DayOfWeek)
            .IsRequired();

        builder.Property(e => e.OpenTime)
            .IsRequired()
            .HasColumnType("time");

        builder.Property(e => e.CloseTime)
            .IsRequired()
            .HasColumnType("time");

        builder.Property(e => e.IsClosed)
            .HasDefaultValue(false);

        // ───────────────────────── Relationships ─────────────────────────

        builder.HasOne(e => e.Restaurant)
            .WithMany(r => r.OperatingHours)
            .HasForeignKey(e => e.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        // ───────────────────────── Indexes ─────────────────────────

        builder.HasIndex(e => new { e.RestaurantId, e.DayOfWeek })
            .IsUnique();
    }
}
