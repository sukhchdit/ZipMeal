using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class ActivityFeedItemConfiguration : IEntityTypeConfiguration<ActivityFeedItem>
{
    public void Configure(EntityTypeBuilder<ActivityFeedItem> builder)
    {
        builder.ToTable("activity_feed_items");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.ActivityType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(e => e.Metadata)
            .HasColumnType("jsonb");

        builder.HasIndex(e => new { e.UserId, e.CreatedAt })
            .IsDescending(false, true);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
