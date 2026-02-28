using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class MenuItemVariantConfiguration : IEntityTypeConfiguration<MenuItemVariant>
{
    public void Configure(EntityTypeBuilder<MenuItemVariant> builder)
    {
        builder.ToTable("menu_item_variants");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.PriceAdjustment)
            .HasDefaultValue(0);

        builder.Property(e => e.IsDefault)
            .HasDefaultValue(false);

        builder.Property(e => e.IsAvailable)
            .HasDefaultValue(true);

        builder.Property(e => e.SortOrder)
            .HasDefaultValue(0);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        // ───────────────────────── Relationships ─────────────────────────

        builder.HasOne(e => e.MenuItem)
            .WithMany(mi => mi.Variants)
            .HasForeignKey(e => e.MenuItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
