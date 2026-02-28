using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.ToTable("coupons");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasColumnType("text");

        builder.Property(e => e.DiscountType)
            .HasConversion<short>();

        builder.Property(e => e.DiscountValue)
            .IsRequired();

        builder.Property(e => e.MinOrderAmount)
            .HasDefaultValue(0);

        builder.Property(e => e.ValidFrom)
            .IsRequired();

        builder.Property(e => e.ValidUntil)
            .IsRequired();

        builder.Property(e => e.MaxUsagesPerUser)
            .HasDefaultValue(1);

        builder.Property(e => e.CurrentUsages)
            .HasDefaultValue(0);

        builder.Property(e => e.ApplicableOrderTypes)
            .HasColumnType("smallint[]");

        builder.Property(e => e.RestaurantIds)
            .HasColumnType("uuid[]");

        builder.Property(e => e.IsActive)
            .HasDefaultValue(true);

        // ───────────────────────── Indexes ─────────────────────────

        builder.HasIndex(e => e.Code)
            .IsUnique()
            .HasDatabaseName("idx_coupons_code")
            .HasFilter("\"IsActive\" = true");
    }
}
