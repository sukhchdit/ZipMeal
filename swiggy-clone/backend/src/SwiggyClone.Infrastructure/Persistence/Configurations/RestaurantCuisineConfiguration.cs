using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class RestaurantCuisineConfiguration : IEntityTypeConfiguration<RestaurantCuisine>
{
    public void Configure(EntityTypeBuilder<RestaurantCuisine> builder)
    {
        builder.ToTable("restaurant_cuisines");

        builder.HasKey(e => new { e.RestaurantId, e.CuisineId });

        // ───────────────────────── Relationships ─────────────────────────

        builder.HasOne(e => e.Restaurant)
            .WithMany(r => r.RestaurantCuisines)
            .HasForeignKey(e => e.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.CuisineType)
            .WithMany(ct => ct.RestaurantCuisines)
            .HasForeignKey(e => e.CuisineId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
