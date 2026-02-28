using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
{
    public void Configure(EntityTypeBuilder<Restaurant> builder)
    {
        builder.ToTable("restaurants");

        builder.HasKey(e => e.Id);

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Slug)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasColumnType("text");

        builder.Property(e => e.CuisineTypes)
            .HasMaxLength(500);

        builder.Property(e => e.PhoneNumber)
            .IsRequired()
            .HasMaxLength(15);

        builder.Property(e => e.Email)
            .HasMaxLength(255);

        builder.Property(e => e.LogoUrl)
            .HasMaxLength(500);

        builder.Property(e => e.BannerUrl)
            .HasMaxLength(500);

        builder.Property(e => e.AddressLine1)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.AddressLine2)
            .HasMaxLength(255);

        builder.Property(e => e.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.State)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.PostalCode)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(e => e.AverageRating)
            .HasColumnType("decimal(2,1)")
            .HasDefaultValue(0.0m);

        builder.Property(e => e.TotalRatings)
            .HasDefaultValue(0);

        builder.Property(e => e.IsVegOnly)
            .HasDefaultValue(false);

        builder.Property(e => e.IsAcceptingOrders)
            .HasDefaultValue(true);

        builder.Property(e => e.IsDineInEnabled)
            .HasDefaultValue(false);

        builder.Property(e => e.CommissionRate)
            .HasColumnType("decimal(5,2)")
            .HasDefaultValue(15.00m);

        builder.Property(e => e.FssaiLicense)
            .HasMaxLength(20);

        builder.Property(e => e.GstNumber)
            .HasMaxLength(20);

        builder.Property(e => e.Status)
            .HasDefaultValue(RestaurantStatus.Pending)
            .HasConversion<short>();

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // ───────────────────────── Relationships ─────────────────────────

        builder.HasOne(e => e.Owner)
            .WithMany()
            .HasForeignKey(e => e.OwnerId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(e => e.OperatingHours)
            .WithOne(oh => oh.Restaurant)
            .HasForeignKey(oh => oh.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.MenuCategories)
            .WithOne(mc => mc.Restaurant)
            .HasForeignKey(mc => mc.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.MenuItems)
            .WithOne(mi => mi.Restaurant)
            .HasForeignKey(mi => mi.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Orders)
            .WithOne(o => o.Restaurant)
            .HasForeignKey(o => o.RestaurantId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(e => e.Tables)
            .WithOne(t => t.Restaurant)
            .HasForeignKey(t => t.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Reviews)
            .WithOne(r => r.Restaurant)
            .HasForeignKey(r => r.RestaurantId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(e => e.RestaurantCuisines)
            .WithOne(rc => rc.Restaurant)
            .HasForeignKey(rc => rc.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        // ───────────────────────── Indexes ─────────────────────────

        builder.HasIndex(e => e.Slug)
            .IsUnique()
            .HasDatabaseName("idx_restaurants_slug");

        builder.HasIndex(e => e.OwnerId)
            .HasDatabaseName("idx_restaurants_owner")
            .HasFilter("\"IsDeleted\" = false");

        builder.HasIndex(e => e.City)
            .HasDatabaseName("idx_restaurants_city")
            .HasFilter("\"IsDeleted\" = false AND \"Status\" = 1");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("idx_restaurants_status")
            .HasFilter("\"IsDeleted\" = false");
    }
}
