using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class LoyaltyTransactionConfiguration : IEntityTypeConfiguration<LoyaltyTransaction>
{
    public void Configure(EntityTypeBuilder<LoyaltyTransaction> builder)
    {
        builder.ToTable("loyalty_transactions");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.Points)
            .IsRequired();

        builder.Property(e => e.Type)
            .HasConversion<short>();

        builder.Property(e => e.Source)
            .HasConversion<short>();

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.BalanceAfter)
            .IsRequired();

        // ───────────────────────── Indexes ─────────────────────────

        builder.HasIndex(e => new { e.LoyaltyAccountId, e.CreatedAt })
            .IsDescending(false, true)
            .HasDatabaseName("idx_loyalty_txn_account_created");

        builder.HasIndex(e => e.ReferenceId)
            .HasDatabaseName("idx_loyalty_txn_reference")
            .HasFilter("\"reference_id\" IS NOT NULL");
    }
}
