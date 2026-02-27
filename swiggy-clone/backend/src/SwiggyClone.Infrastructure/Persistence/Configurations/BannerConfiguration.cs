using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class BannerConfiguration : IEntityTypeConfiguration<Banner>
{
    public void Configure(EntityTypeBuilder<Banner> builder)
    {
        builder.ToTable("banners");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.DeepLink)
            .HasMaxLength(500);

        builder.Property(e => e.DisplayOrder)
            .HasDefaultValue(0);

        builder.Property(e => e.IsActive)
            .HasDefaultValue(true);

        builder.Property(e => e.ValidFrom)
            .IsRequired();

        builder.Property(e => e.ValidUntil)
            .IsRequired();

        // ───────────────────────── Indexes ─────────────────────────

        builder.HasIndex(e => new { e.IsActive, e.DisplayOrder })
            .HasDatabaseName("idx_banners_active_order");

        builder.HasIndex(e => new { e.ValidFrom, e.ValidUntil })
            .HasDatabaseName("idx_banners_validity");
    }
}
