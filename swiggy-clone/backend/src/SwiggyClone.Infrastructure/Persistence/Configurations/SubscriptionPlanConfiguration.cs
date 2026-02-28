using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.ToTable("subscription_plans");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Description)
            .HasColumnType("text");

        builder.Property(e => e.PricePaise)
            .IsRequired();

        builder.Property(e => e.DurationDays)
            .IsRequired();

        builder.Property(e => e.FreeDelivery)
            .HasDefaultValue(false);

        builder.Property(e => e.ExtraDiscountPercent)
            .HasDefaultValue(0);

        builder.Property(e => e.NoSurgeFee)
            .HasDefaultValue(false);

        builder.Property(e => e.IsActive)
            .HasDefaultValue(true);

        // ───────────────────────── Relationships ─────────────────────────

        builder.HasMany(e => e.Subscriptions)
            .WithOne(s => s.Plan)
            .HasForeignKey(s => s.PlanId);

        // ───────────────────────── Indexes ─────────────────────────

        builder.HasIndex(e => e.IsActive)
            .HasDatabaseName("idx_subscription_plans_active");
    }
}
