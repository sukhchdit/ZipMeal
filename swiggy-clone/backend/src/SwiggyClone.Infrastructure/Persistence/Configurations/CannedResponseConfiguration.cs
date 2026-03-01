using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class CannedResponseConfiguration : IEntityTypeConfiguration<CannedResponse>
{
    public void Configure(EntityTypeBuilder<CannedResponse> builder)
    {
        builder.ToTable("canned_responses");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Content)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(e => e.Category)
            .IsRequired()
            .HasConversion<short>();

        builder.Property(e => e.IsActive)
            .HasDefaultValue(true);

        // Indexes
        builder.HasIndex(e => new { e.Category, e.SortOrder })
            .HasDatabaseName("idx_canned_responses_category_sort")
            .HasFilter("\"IsDeleted\" = false AND \"IsActive\" = true");
    }
}
