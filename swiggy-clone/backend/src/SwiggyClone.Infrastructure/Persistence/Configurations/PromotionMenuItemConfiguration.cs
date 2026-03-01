using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class PromotionMenuItemConfiguration : IEntityTypeConfiguration<PromotionMenuItem>
{
    public void Configure(EntityTypeBuilder<PromotionMenuItem> builder)
    {
        builder.ToTable("promotion_menu_items");
        builder.HasKey(e => new { e.PromotionId, e.MenuItemId });

        builder.Property(e => e.Quantity).HasDefaultValue(1);

        builder.HasOne(e => e.Promotion).WithMany(p => p.PromotionMenuItems)
            .HasForeignKey(e => e.PromotionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.MenuItem).WithMany()
            .HasForeignKey(e => e.MenuItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
