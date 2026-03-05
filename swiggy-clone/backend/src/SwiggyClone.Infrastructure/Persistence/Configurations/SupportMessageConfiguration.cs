using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class SupportMessageConfiguration : IEntityTypeConfiguration<SupportMessage>
{
    public void Configure(EntityTypeBuilder<SupportMessage> builder)
    {
        builder.ToTable("support_messages");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Content)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(e => e.MessageType)
            .IsRequired()
            .HasConversion<short>();

        builder.Property(e => e.IsRead)
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(e => e.Ticket)
            .WithMany(t => t.Messages)
            .HasForeignKey(e => e.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Sender)
            .WithMany()
            .HasForeignKey(e => e.SenderId)
            .OnDelete(DeleteBehavior.NoAction);

        // Indexes
        builder.HasIndex(e => new { e.TicketId, e.CreatedAt })
            .IsDescending(false, true)
            .HasDatabaseName("idx_support_messages_ticket");

        builder.HasIndex(e => new { e.TicketId, e.IsRead })
            .HasDatabaseName("idx_support_messages_unread")
            .HasFilter("\"is_read\" = false AND \"is_deleted\" = false");
    }
}
