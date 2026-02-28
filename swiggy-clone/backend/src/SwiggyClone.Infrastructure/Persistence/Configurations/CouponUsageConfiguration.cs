using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class CouponUsageConfiguration : IEntityTypeConfiguration<CouponUsage>
{
    public void Configure(EntityTypeBuilder<CouponUsage> builder)
    {
        builder.ToTable("coupon_usages");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.DiscountAmount)
            .IsRequired();

        builder.Property(e => e.UsedAt)
            .IsRequired();

        // ───────────────────────── Relationships ─────────────────────────

        builder.HasOne(e => e.Coupon)
            .WithMany()
            .HasForeignKey(e => e.CouponId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.Order)
            .WithMany()
            .HasForeignKey(e => e.OrderId)
            .OnDelete(DeleteBehavior.NoAction);

        // ───────────────────────── Indexes ─────────────────────────

        builder.HasIndex(e => new { e.CouponId, e.UserId })
            .HasDatabaseName("idx_coupon_usages_user");
    }
}
