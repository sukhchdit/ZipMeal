using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class UserFavoriteItemConfiguration : IEntityTypeConfiguration<UserFavoriteItem>
{
    public void Configure(EntityTypeBuilder<UserFavoriteItem> builder)
    {
        builder.ToTable("user_favorite_items");

        builder.HasKey(e => new { e.UserId, e.MenuItemId });

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        // ───────────────────────── Indexes ─────────────────────────

        builder.HasIndex(e => new { e.UserId, e.CreatedAt })
            .IsDescending(false, true);

        // ───────────────────────── Relationships ─────────────────────────

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.MenuItem)
            .WithMany()
            .HasForeignKey(e => e.MenuItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
