using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> builder)
    {
        builder.ToTable("user_subscriptions");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.Status)
            .HasConversion<short>()
            .HasDefaultValue(SubscriptionStatus.Active);

        builder.Property(e => e.PaidAmountPaise)
            .IsRequired();

        // ───────────────────────── Relationships ─────────────────────────

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.Plan)
            .WithMany(p => p.Subscriptions)
            .HasForeignKey(e => e.PlanId)
            .OnDelete(DeleteBehavior.NoAction);

        // ───────────────────────── Indexes ─────────────────────────

        builder.HasIndex(e => new { e.UserId, e.Status })
            .HasDatabaseName("idx_user_sub_user_status");

        builder.HasIndex(e => e.EndDate)
            .HasDatabaseName("idx_user_sub_end_date");
    }
}
