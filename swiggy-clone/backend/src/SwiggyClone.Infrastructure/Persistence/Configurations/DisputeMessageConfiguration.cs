using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class DisputeMessageConfiguration : IEntityTypeConfiguration<DisputeMessage>
{
    public void Configure(EntityTypeBuilder<DisputeMessage> builder)
    {
        builder.ToTable("dispute_messages");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Content)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(e => e.IsSystemMessage)
            .HasDefaultValue(false);

        builder.Property(e => e.IsRead)
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(e => e.Dispute)
            .WithMany(d => d.Messages)
            .HasForeignKey(e => e.DisputeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Sender)
            .WithMany()
            .HasForeignKey(e => e.SenderId)
            .OnDelete(DeleteBehavior.NoAction);

        // Indexes
        builder.HasIndex(e => new { e.DisputeId, e.CreatedAt })
            .IsDescending(false, true)
            .HasDatabaseName("idx_dispute_messages_dispute");

        builder.HasIndex(e => new { e.DisputeId, e.IsRead })
            .HasDatabaseName("idx_dispute_messages_unread")
            .HasFilter("\"is_read\" = false AND \"is_deleted\" = false");
    }
}
