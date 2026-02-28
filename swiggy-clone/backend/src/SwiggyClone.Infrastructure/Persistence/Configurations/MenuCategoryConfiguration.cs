using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class MenuCategoryConfiguration : IEntityTypeConfiguration<MenuCategory>
{
    public void Configure(EntityTypeBuilder<MenuCategory> builder)
    {
        builder.ToTable("menu_categories");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.SortOrder)
            .HasDefaultValue(0);

        builder.Property(e => e.IsActive)
            .HasDefaultValue(true);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired();

        // ───────────────────────── Relationships ─────────────────────────

        builder.HasOne(e => e.Restaurant)
            .WithMany(r => r.MenuCategories)
            .HasForeignKey(e => e.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        // ───────────────────────── Indexes ─────────────────────────

        builder.HasIndex(e => e.RestaurantId)
            .HasDatabaseName("idx_menu_categories_restaurant")
            .HasFilter("\"IsActive\" = true");
    }
}
