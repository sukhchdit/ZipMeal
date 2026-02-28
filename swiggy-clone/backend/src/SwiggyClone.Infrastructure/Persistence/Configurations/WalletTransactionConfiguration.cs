using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
{
    public void Configure(EntityTypeBuilder<WalletTransaction> builder)
    {
        builder.ToTable("wallet_transactions");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.AmountPaise)
            .IsRequired();

        builder.Property(e => e.Type)
            .HasConversion<short>();

        builder.Property(e => e.Source)
            .HasConversion<short>();

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.BalanceAfterPaise)
            .IsRequired();

        // ───────────────────────── Indexes ─────────────────────────

        builder.HasIndex(e => new { e.WalletId, e.CreatedAt })
            .IsDescending(false, true)
            .HasDatabaseName("idx_wallet_txn_wallet_created");

        builder.HasIndex(e => e.ReferenceId)
            .HasDatabaseName("idx_wallet_txn_reference")
            .HasFilter("\"ReferenceId\" IS NOT NULL");
    }
}
