using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class LoyaltyTierConfiguration : IEntityTypeConfiguration<LoyaltyTier>
{
    public void Configure(EntityTypeBuilder<LoyaltyTier> builder)
    {
        builder.ToTable("loyalty_tiers");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Level)
            .HasConversion<short>();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.MinLifetimePoints)
            .IsRequired();

        builder.Property(e => e.PointsPerHundredPaise)
            .IsRequired();

        builder.Property(e => e.Multiplier)
            .IsRequired();

        // ───────────────────────── Indexes ─────────────────────────

        builder.HasIndex(e => e.Level)
            .IsUnique()
            .HasDatabaseName("idx_loyalty_tiers_level");

        // ───────────────────────── Seed Data ─────────────────────────

        builder.HasData(
            new LoyaltyTier
            {
                Id = Guid.Parse("a0000000-0000-0000-0000-000000000001"),
                Level = LoyaltyTierLevel.Bronze,
                Name = "Bronze",
                MinLifetimePoints = 0,
                PointsPerHundredPaise = 1,
                Multiplier = 1.0,
            },
            new LoyaltyTier
            {
                Id = Guid.Parse("a0000000-0000-0000-0000-000000000002"),
                Level = LoyaltyTierLevel.Silver,
                Name = "Silver",
                MinLifetimePoints = 500,
                PointsPerHundredPaise = 1,
                Multiplier = 1.5,
            },
            new LoyaltyTier
            {
                Id = Guid.Parse("a0000000-0000-0000-0000-000000000003"),
                Level = LoyaltyTierLevel.Gold,
                Name = "Gold",
                MinLifetimePoints = 2000,
                PointsPerHundredPaise = 1,
                Multiplier = 2.0,
            },
            new LoyaltyTier
            {
                Id = Guid.Parse("a0000000-0000-0000-0000-000000000004"),
                Level = LoyaltyTierLevel.Platinum,
                Name = "Platinum",
                MinLifetimePoints = 5000,
                PointsPerHundredPaise = 1,
                Multiplier = 3.0,
            });
    }
}
