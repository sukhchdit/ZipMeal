using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        builder.HasKey(e => e.Id);

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.OrderNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.OrderType)
            .IsRequired()
            .HasConversion<short>();

        builder.Property(e => e.Status)
            .IsRequired()
            .HasDefaultValue(OrderStatus.Placed)
            .HasConversion<short>();

        builder.Property(e => e.Subtotal)
            .IsRequired();

        builder.Property(e => e.TaxAmount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(e => e.DeliveryFee)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(e => e.PackagingCharge)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(e => e.DiscountAmount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(e => e.TotalAmount)
            .IsRequired();

        builder.Property(e => e.PaymentStatus)
            .HasDefaultValue(PaymentStatus.Pending)
            .HasConversion<short>();

        builder.Property(e => e.PaymentMethod)
            .HasConversion<short?>();

        builder.Property(e => e.SpecialInstructions)
            .HasColumnType("text");

        builder.Property(e => e.CancellationReason)
            .HasMaxLength(500);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // ───────────────────────── Relationships ─────────────────────────

        builder.HasOne(e => e.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.Restaurant)
            .WithMany(r => r.Orders)
            .HasForeignKey(e => e.RestaurantId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.DeliveryAddress)
            .WithMany()
            .HasForeignKey(e => e.DeliveryAddressId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.DeliveryPartner)
            .WithMany()
            .HasForeignKey(e => e.DeliveryPartnerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.DineInTable)
            .WithMany()
            .HasForeignKey(e => e.DineInTableId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.DineInSession)
            .WithMany(s => s.Orders)
            .HasForeignKey(e => e.DineInSessionId)
            .OnDelete(DeleteBehavior.SetNull);

        // ───────────────────────── Indexes ─────────────────────────

        builder.HasIndex(e => e.OrderNumber)
            .IsUnique();

        builder.HasIndex(e => new { e.UserId, e.CreatedAt })
            .IsDescending(false, true)
            .HasDatabaseName("idx_orders_user")
            .HasFilter("\"is_deleted\" = false");

        builder.HasIndex(e => new { e.RestaurantId, e.CreatedAt })
            .IsDescending(false, true)
            .HasDatabaseName("idx_orders_restaurant")
            .HasFilter("\"is_deleted\" = false");

        builder.HasIndex(e => new { e.RestaurantId, e.Status })
            .HasDatabaseName("idx_orders_status")
            .HasFilter("\"is_deleted\" = false");

        builder.HasIndex(e => e.DeliveryPartnerId)
            .HasDatabaseName("idx_orders_delivery_partner")
            .HasFilter("\"delivery_partner_id\" IS NOT NULL AND \"is_deleted\" = false");

        builder.HasIndex(e => new { e.OrderType, e.Status })
            .HasDatabaseName("idx_orders_type_status")
            .HasFilter("\"is_deleted\" = false");

        builder.HasIndex(e => e.DineInTableId)
            .HasDatabaseName("idx_orders_dine_in_table")
            .HasFilter("\"dine_in_table_id\" IS NOT NULL");

        builder.HasIndex(e => e.DineInSessionId)
            .HasDatabaseName("idx_orders_dine_in_session")
            .HasFilter("\"dine_in_session_id\" IS NOT NULL");

        builder.HasOne(e => e.GroupOrder)
            .WithMany()
            .HasForeignKey(e => e.GroupOrderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(e => e.GroupOrderId)
            .HasDatabaseName("idx_orders_group_order")
            .HasFilter("\"group_order_id\" IS NOT NULL");

        builder.HasIndex(e => new { e.Status, e.ScheduledDeliveryTime })
            .HasDatabaseName("idx_orders_scheduled")
            .HasFilter("\"status\" = 7 AND \"scheduled_delivery_time\" IS NOT NULL AND \"is_deleted\" = false");
    }
}
