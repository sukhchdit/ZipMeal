using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class DineInSessionConfiguration : IEntityTypeConfiguration<DineInSession>
{
    public void Configure(EntityTypeBuilder<DineInSession> builder)
    {
        builder.ToTable("dine_in_sessions");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.SessionCode)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(e => e.Status)
            .HasDefaultValue(DineInSessionStatus.Active)
            .HasConversion<short>();

        builder.Property(e => e.GuestCount)
            .HasDefaultValue(1);

        builder.Property(e => e.StartedAt)
            .IsRequired();

        // ───────────────────────── Relationships ─────────────────────────

        builder.HasOne(e => e.Restaurant)
            .WithMany()
            .HasForeignKey(e => e.RestaurantId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.Table)
            .WithMany(t => t.DineInSessions)
            .HasForeignKey(e => e.TableId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.Customer)
            .WithMany()
            .HasForeignKey(e => e.CustomerId)
            .OnDelete(DeleteBehavior.NoAction);

        // ───────────────────────── Indexes ─────────────────────────

        builder.HasIndex(e => e.SessionCode)
            .IsUnique();

        builder.HasIndex(e => new { e.TableId, e.Status })
            .HasDatabaseName("idx_dine_in_sessions_table");

        builder.HasIndex(e => new { e.CustomerId, e.CreatedAt })
            .IsDescending(false, true)
            .HasDatabaseName("idx_dine_in_sessions_customer");
    }
}
