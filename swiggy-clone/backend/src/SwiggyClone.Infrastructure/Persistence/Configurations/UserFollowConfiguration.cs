using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class UserFollowConfiguration : IEntityTypeConfiguration<UserFollow>
{
    public void Configure(EntityTypeBuilder<UserFollow> builder)
    {
        builder.ToTable("user_follows");

        builder.HasKey(e => new { e.FollowerId, e.FollowingId });

        builder.Property(e => e.CreatedAt).IsRequired();

        builder.HasIndex(e => new { e.FollowingId, e.CreatedAt })
            .IsDescending(false, true);

        builder.ToTable(t => t.HasCheckConstraint(
            "ck_user_follows_no_self_follow",
            "\"follower_id\" <> \"following_id\""));

        builder.HasOne(e => e.Follower)
            .WithMany()
            .HasForeignKey(e => e.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Following)
            .WithMany()
            .HasForeignKey(e => e.FollowingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
