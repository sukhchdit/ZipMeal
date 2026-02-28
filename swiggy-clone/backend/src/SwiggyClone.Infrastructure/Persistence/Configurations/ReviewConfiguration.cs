using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.Rating)
            .IsRequired();

        builder.Property(e => e.ReviewText)
            .HasColumnType("text");

        builder.Property(e => e.IsAnonymous)
            .HasDefaultValue(false);

        builder.Property(e => e.IsVisible)
            .HasDefaultValue(true);

        builder.Property(e => e.RestaurantReply)
            .HasColumnType("text");

        // ───────────────────────── Relationships ─────────────────────────

        builder.HasOne(e => e.Order)
            .WithMany()
            .HasForeignKey(e => e.OrderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.Restaurant)
            .WithMany(r => r.Reviews)
            .HasForeignKey(e => e.RestaurantId)
            .OnDelete(DeleteBehavior.NoAction);

        // ───────────────────────── Indexes ─────────────────────────

        builder.HasIndex(e => e.OrderId)
            .IsUnique();

        builder.HasIndex(e => new { e.RestaurantId, e.CreatedAt })
            .IsDescending(false, true)
            .HasDatabaseName("idx_reviews_restaurant")
            .HasFilter("\"IsVisible\" = true");

        builder.HasIndex(e => new { e.UserId, e.CreatedAt })
            .IsDescending(false, true)
            .HasDatabaseName("idx_reviews_user");
    }
}
