using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class OrderItemAddonConfiguration : IEntityTypeConfiguration<OrderItemAddon>
{
    public void Configure(EntityTypeBuilder<OrderItemAddon> builder)
    {
        builder.ToTable("order_item_addons");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.AddonName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Quantity)
            .HasDefaultValue(1);

        builder.Property(e => e.Price)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        // ───────────────────────── Relationships ─────────────────────────

        builder.HasOne(e => e.OrderItem)
            .WithMany(oi => oi.Addons)
            .HasForeignKey(e => e.OrderItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Addon)
            .WithMany()
            .HasForeignKey(e => e.AddonId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
