using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class ReviewReportConfiguration : IEntityTypeConfiguration<ReviewReport>
{
    public void Configure(EntityTypeBuilder<ReviewReport> builder)
    {
        builder.ToTable("review_reports");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.Reason).IsRequired();
        builder.Property(e => e.Description).HasColumnType("text");
        builder.Property(e => e.Status).HasDefaultValue(ReviewReportStatus.Pending);
        builder.Property(e => e.AdminNotes).HasColumnType("text");
        builder.Property(e => e.CreatedAt).IsRequired();

        // Unique: one report per user per review
        builder.HasIndex(e => new { e.ReviewId, e.UserId })
            .IsUnique()
            .HasDatabaseName("idx_review_reports_review_user");

        builder.HasIndex(e => new { e.Status, e.CreatedAt })
            .IsDescending(false, true)
            .HasDatabaseName("idx_review_reports_status");

        builder.HasOne(e => e.Review)
            .WithMany(r => r.Reports)
            .HasForeignKey(e => e.ReviewId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.ResolvedByAdmin)
            .WithMany()
            .HasForeignKey(e => e.ResolvedByAdminId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
