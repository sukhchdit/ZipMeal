using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class UserDeviceConfiguration : IEntityTypeConfiguration<UserDevice>
{
    public void Configure(EntityTypeBuilder<UserDevice> builder)
    {
        builder.ToTable("user_devices");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.DeviceToken)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(e => e.Platform)
            .HasConversion<short>();

        builder.Property(e => e.IsActive)
            .HasDefaultValue(true);

        // ───────────────────────── Relationships ─────────────────────────

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ───────────────────────── Indexes ─────────────────────────

        builder.HasIndex(e => new { e.UserId, e.DeviceToken })
            .IsUnique();
    }
}
