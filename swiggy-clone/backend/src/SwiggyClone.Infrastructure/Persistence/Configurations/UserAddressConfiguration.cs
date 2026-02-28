using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class UserAddressConfiguration : IEntityTypeConfiguration<UserAddress>
{
    public void Configure(EntityTypeBuilder<UserAddress> builder)
    {
        builder.ToTable("user_addresses");

        builder.HasKey(e => e.Id);

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.Label)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.AddressLine1)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.AddressLine2)
            .HasMaxLength(255);

        builder.Property(e => e.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.State)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.PostalCode)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(e => e.Country)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("India");

        builder.Property(e => e.Latitude)
            .IsRequired();

        builder.Property(e => e.Longitude)
            .IsRequired();

        builder.Property(e => e.IsDefault)
            .HasDefaultValue(false);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // ───────────────────────── Relationships ─────────────────────────

        builder.HasOne(e => e.User)
            .WithMany(u => u.Addresses)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
