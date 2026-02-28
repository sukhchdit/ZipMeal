using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.ToTable("menu_items");

        builder.HasKey(e => e.Id);

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasColumnType("text");

        builder.Property(e => e.Price)
            .IsRequired();

        builder.Property(e => e.ImageUrl)
            .HasMaxLength(500);

        builder.Property(e => e.IsVeg)
            .HasDefaultValue(true);

        builder.Property(e => e.IsAvailable)
            .HasDefaultValue(true);

        builder.Property(e => e.IsBestseller)
            .HasDefaultValue(false);

        builder.Property(e => e.PreparationTimeMin)
            .HasDefaultValue(15);

        builder.Property(e => e.SortOrder)
            .HasDefaultValue(0);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // ───────────────────────── Relationships ─────────────────────────

        builder.HasOne(e => e.Category)
            .WithMany(mc => mc.MenuItems)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Restaurant)
            .WithMany(r => r.MenuItems)
            .HasForeignKey(e => e.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Variants)
            .WithOne(v => v.MenuItem)
            .HasForeignKey(v => v.MenuItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Addons)
            .WithOne(a => a.MenuItem)
            .HasForeignKey(a => a.MenuItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // ───────────────────────── Indexes ─────────────────────────

        builder.HasIndex(e => e.RestaurantId)
            .HasDatabaseName("idx_menu_items_restaurant")
            .HasFilter("\"IsDeleted\" = false");

        builder.HasIndex(e => e.CategoryId)
            .HasDatabaseName("idx_menu_items_category")
            .HasFilter("\"IsDeleted\" = false");
    }
}
