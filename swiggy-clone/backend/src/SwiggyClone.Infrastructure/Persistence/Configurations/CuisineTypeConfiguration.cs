using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class CuisineTypeConfiguration : IEntityTypeConfiguration<CuisineType>
{
    public void Configure(EntityTypeBuilder<CuisineType> builder)
    {
        builder.ToTable("cuisine_types");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.IconUrl)
            .HasMaxLength(500);

        builder.Property(e => e.SortOrder)
            .HasDefaultValue(0);

        builder.Property(e => e.IsActive)
            .HasDefaultValue(true);

        // ───────────────────────── Indexes ─────────────────────────

        builder.HasIndex(e => e.Name)
            .IsUnique();
    }
}
