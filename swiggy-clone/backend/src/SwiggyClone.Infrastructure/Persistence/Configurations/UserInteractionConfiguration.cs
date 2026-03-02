using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class UserInteractionConfiguration : IEntityTypeConfiguration<UserInteraction>
{
    public void Configure(EntityTypeBuilder<UserInteraction> builder)
    {
        builder.ToTable("user_interactions");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.UserId).IsRequired();
        builder.Property(e => e.EntityType).IsRequired();
        builder.Property(e => e.EntityId).IsRequired();
        builder.Property(e => e.InteractionType).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();

        builder.HasIndex(e => new { e.UserId, e.EntityType, e.EntityId, e.CreatedAt })
            .IsDescending(false, false, false, true)
            .HasDatabaseName("ix_user_interactions_user_entity_created");

        builder.HasIndex(e => new { e.EntityType, e.InteractionType, e.CreatedAt })
            .IsDescending(false, false, true)
            .HasDatabaseName("ix_user_interactions_trending");

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
