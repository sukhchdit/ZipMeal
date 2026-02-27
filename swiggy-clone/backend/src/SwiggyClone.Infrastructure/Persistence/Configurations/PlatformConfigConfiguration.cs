using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class PlatformConfigConfiguration : IEntityTypeConfiguration<PlatformConfig>
{
    public void Configure(EntityTypeBuilder<PlatformConfig> builder)
    {
        builder.ToTable("platform_config");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.DeliveryFeePaise)
            .IsRequired()
            .HasDefaultValue(4900);

        builder.Property(e => e.PackagingChargePaise)
            .IsRequired()
            .HasDefaultValue(1500);

        builder.Property(e => e.TaxRatePercent)
            .IsRequired()
            .HasPrecision(5, 2)
            .HasDefaultValue(5.00m);

        builder.Property(e => e.FreeDeliveryThresholdPaise);

        // ───────────────────────── Seed Data ─────────────────────────

        builder.HasData(new PlatformConfig
        {
            Id = Guid.Parse("019508a0-0000-7000-8000-000000000001"),
            DeliveryFeePaise = 4900,
            PackagingChargePaise = 1500,
            TaxRatePercent = 5.00m,
            FreeDeliveryThresholdPaise = null,
            CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
        });
    }
}
