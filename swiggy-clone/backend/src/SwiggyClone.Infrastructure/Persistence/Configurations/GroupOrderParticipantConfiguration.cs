using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class GroupOrderParticipantConfiguration : IEntityTypeConfiguration<GroupOrderParticipant>
{
    public void Configure(EntityTypeBuilder<GroupOrderParticipant> builder)
    {
        builder.ToTable("group_order_participants");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.IsInitiator)
            .HasDefaultValue(false);

        builder.Property(e => e.Status)
            .HasDefaultValue(GroupOrderParticipantStatus.Joined)
            .HasConversion<short>();

        builder.Property(e => e.JoinedAt)
            .IsRequired();

        builder.Property(e => e.ItemCount)
            .HasDefaultValue(0);

        builder.Property(e => e.ItemsTotal)
            .HasDefaultValue(0);

        // ───────────────────────── Relationships ─────────────────────────

        builder.HasOne(e => e.GroupOrder)
            .WithMany(g => g.Participants)
            .HasForeignKey(e => e.GroupOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ───────────────────────── Indexes ─────────────────────────

        builder.HasIndex(e => new { e.GroupOrderId, e.UserId })
            .IsUnique()
            .HasDatabaseName("idx_group_order_participants_unique");
    }
}
