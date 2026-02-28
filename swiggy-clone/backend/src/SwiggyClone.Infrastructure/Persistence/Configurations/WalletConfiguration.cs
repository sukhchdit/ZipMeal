using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.ToTable("wallets");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.BalancePaise)
            .IsRequired()
            .HasDefaultValue(0);

        // ───────────────────────── Relationships ─────────────────────────

        builder.HasOne(e => e.User)
            .WithOne()
            .HasForeignKey<Wallet>(e => e.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(e => e.Transactions)
            .WithOne(t => t.Wallet)
            .HasForeignKey(t => t.WalletId);

        // ───────────────────────── Indexes ─────────────────────────

        builder.HasIndex(e => e.UserId)
            .IsUnique()
            .HasDatabaseName("idx_wallets_user_id");
    }
}
