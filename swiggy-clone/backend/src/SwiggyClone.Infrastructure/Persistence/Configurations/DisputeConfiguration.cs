using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class DisputeConfiguration : IEntityTypeConfiguration<Dispute>
{
    public void Configure(EntityTypeBuilder<Dispute> builder)
    {
        builder.ToTable("disputes");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.DisputeNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.IssueType)
            .IsRequired()
            .HasConversion<short>();

        builder.Property(e => e.Status)
            .IsRequired()
            .HasDefaultValue(DisputeStatus.Opened)
            .HasConversion<short>();

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(e => e.ResolutionType)
            .HasConversion<short?>();

        builder.Property(e => e.ResolutionNotes)
            .HasMaxLength(2000);

        builder.Property(e => e.RejectionReason)
            .HasMaxLength(2000);

        // Relationships
        builder.HasOne(e => e.Order)
            .WithMany()
            .HasForeignKey(e => e.OrderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.AssignedAgent)
            .WithMany()
            .HasForeignKey(e => e.AssignedAgentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.ResolvedByAgent)
            .WithMany()
            .HasForeignKey(e => e.ResolvedByAgentId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(e => e.DisputeNumber)
            .IsUnique()
            .HasDatabaseName("idx_disputes_number");

        builder.HasIndex(e => new { e.UserId, e.CreatedAt })
            .IsDescending(false, true)
            .HasDatabaseName("idx_disputes_user");

        builder.HasIndex(e => e.OrderId)
            .HasDatabaseName("idx_disputes_order");

        builder.HasIndex(e => new { e.Status, e.CreatedAt })
            .HasDatabaseName("idx_disputes_status")
            .HasFilter("\"is_deleted\" = false");

        builder.HasIndex(e => new { e.AssignedAgentId, e.Status })
            .HasDatabaseName("idx_disputes_agent_status")
            .HasFilter("\"is_deleted\" = false");
    }
}
