using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class MenuItemAddonConfiguration : IEntityTypeConfiguration<MenuItemAddon>
{
    public void Configure(EntityTypeBuilder<MenuItemAddon> builder)
    {
        builder.ToTable("menu_item_addons");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Price)
            .HasDefaultValue(0);

        builder.Property(e => e.IsVeg)
            .HasDefaultValue(true);

        builder.Property(e => e.IsAvailable)
            .HasDefaultValue(true);

        builder.Property(e => e.MaxQuantity)
            .HasDefaultValue(5);

        builder.Property(e => e.SortOrder)
            .HasDefaultValue(0);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        // ───────────────────────── Relationships ─────────────────────────

        builder.HasOne(e => e.MenuItem)
            .WithMany(mi => mi.Addons)
            .HasForeignKey(e => e.MenuItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
