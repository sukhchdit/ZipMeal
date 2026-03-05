using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class LoyaltyAccountConfiguration : IEntityTypeConfiguration<LoyaltyAccount>
{
    public void Configure(EntityTypeBuilder<LoyaltyAccount> builder)
    {
        builder.ToTable("loyalty_accounts");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.PointsBalance)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(e => e.LifetimePointsEarned)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(e => e.CurrentTier)
            .HasConversion<short>();

        // ───────────────────────── Relationships ─────────────────────────

        builder.HasOne(e => e.User)
            .WithOne()
            .HasForeignKey<LoyaltyAccount>(e => e.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(e => e.Transactions)
            .WithOne(t => t.Account)
            .HasForeignKey(t => t.LoyaltyAccountId);

        // ───────────────────────── Indexes ─────────────────────────

        builder.HasIndex(e => e.UserId)
            .IsUnique()
            .HasDatabaseName("idx_loyalty_accounts_user_id");
    }
}
