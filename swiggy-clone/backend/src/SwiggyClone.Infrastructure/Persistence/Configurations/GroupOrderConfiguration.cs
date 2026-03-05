using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class GroupOrderConfiguration : IEntityTypeConfiguration<GroupOrder>
{
    public void Configure(EntityTypeBuilder<GroupOrder> builder)
    {
        builder.ToTable("group_orders");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.InviteCode)
            .IsRequired()
            .HasMaxLength(6);

        builder.Property(e => e.Status)
            .HasDefaultValue(GroupOrderStatus.Active)
            .HasConversion<short>();

        builder.Property(e => e.PaymentSplitType)
            .HasDefaultValue(PaymentSplitType.InitiatorPays)
            .HasConversion<short>();

        builder.Property(e => e.SpecialInstructions)
            .HasMaxLength(500);

        builder.Property(e => e.ExpiresAt)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        // ───────────────────────── Relationships ─────────────────────────

        builder.HasOne(e => e.Restaurant)
            .WithMany()
            .HasForeignKey(e => e.RestaurantId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.InitiatorUser)
            .WithMany()
            .HasForeignKey(e => e.InitiatorUserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.DeliveryAddress)
            .WithMany()
            .HasForeignKey(e => e.DeliveryAddressId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.Order)
            .WithMany()
            .HasForeignKey(e => e.OrderId)
            .OnDelete(DeleteBehavior.SetNull);

        // ───────────────────────── Indexes ─────────────────────────

        builder.HasIndex(e => e.InviteCode)
            .IsUnique();

        builder.HasIndex(e => new { e.InitiatorUserId, e.Status })
            .HasDatabaseName("idx_group_orders_initiator_status");

        builder.HasIndex(e => new { e.Status, e.ExpiresAt })
            .HasDatabaseName("idx_group_orders_expiry")
            .HasFilter("\"status\" = 0");
    }
}
