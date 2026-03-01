using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class RestaurantPromotionConfiguration : IEntityTypeConfiguration<RestaurantPromotion>
{
    public void Configure(EntityTypeBuilder<RestaurantPromotion> builder)
    {
        builder.ToTable("restaurant_promotions");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Description).HasColumnType("text");
        builder.Property(e => e.ImageUrl).HasMaxLength(500);
        builder.Property(e => e.PromotionType).HasConversion<short>().IsRequired();
        builder.Property(e => e.DiscountType).HasConversion<short>().IsRequired();
        builder.Property(e => e.DiscountValue).IsRequired();
        builder.Property(e => e.RecurringDaysOfWeek).HasColumnType("smallint[]");
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.DisplayOrder).HasDefaultValue(0);
        builder.Property(e => e.ValidFrom).IsRequired();
        builder.Property(e => e.ValidUntil).IsRequired();

        builder.HasIndex(e => new { e.RestaurantId, e.IsActive, e.DisplayOrder })
            .HasDatabaseName("idx_promotions_restaurant_active");
        builder.HasIndex(e => new { e.ValidFrom, e.ValidUntil })
            .HasDatabaseName("idx_promotions_validity");

        builder.HasOne(e => e.Restaurant).WithMany()
            .HasForeignKey(e => e.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
