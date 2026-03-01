using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class SupportTicketConfiguration : IEntityTypeConfiguration<SupportTicket>
{
    public void Configure(EntityTypeBuilder<SupportTicket> builder)
    {
        builder.ToTable("support_tickets");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Subject)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Category)
            .IsRequired()
            .HasConversion<short>();

        builder.Property(e => e.Status)
            .IsRequired()
            .HasDefaultValue(SupportTicketStatus.Open)
            .HasConversion<short>();

        // Relationships
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.AssignedAgent)
            .WithMany()
            .HasForeignKey(e => e.AssignedAgentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.Order)
            .WithMany()
            .HasForeignKey(e => e.OrderId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(e => new { e.UserId, e.CreatedAt })
            .IsDescending(false, true)
            .HasDatabaseName("idx_support_tickets_user");

        builder.HasIndex(e => new { e.AssignedAgentId, e.Status })
            .HasDatabaseName("idx_support_tickets_agent_status")
            .HasFilter("\"IsDeleted\" = false");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("idx_support_tickets_status")
            .HasFilter("\"IsDeleted\" = false");
    }
}
