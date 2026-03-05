using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.PaymentGateway)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.GatewayPaymentId)
            .HasMaxLength(255);

        builder.Property(e => e.GatewayOrderId)
            .HasMaxLength(255);

        builder.Property(e => e.GatewaySignature)
            .HasMaxLength(512);

        builder.Property(e => e.Amount)
            .IsRequired();

        builder.Property(e => e.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("INR");

        builder.Property(e => e.Status)
            .HasDefaultValue(PaymentStatus.Pending)
            .HasConversion<short>();

        builder.Property(e => e.Method)
            .HasMaxLength(50);

        builder.Property(e => e.Metadata)
            .HasColumnType("jsonb");

        builder.Property(e => e.RefundAmount)
            .HasDefaultValue(0);

        builder.Property(e => e.RefundReason)
            .HasMaxLength(500);

        // ───────────────────────── Relationships ─────────────────────────

        builder.HasOne(e => e.Order)
            .WithOne(o => o.Payment)
            .HasForeignKey<Payment>(e => e.OrderId)
            .OnDelete(DeleteBehavior.NoAction);

        // ───────────────────────── Indexes ─────────────────────────

        builder.HasIndex(e => e.GatewayPaymentId)
            .IsUnique()
            .HasDatabaseName("idx_payments_gateway_id")
            .HasFilter("\"gateway_payment_id\" IS NOT NULL");

        builder.HasIndex(e => e.OrderId)
            .HasDatabaseName("idx_payments_order");
    }
}
