using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(e => e.Id);

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.PhoneNumber)
            .IsRequired()
            .HasMaxLength(15);

        builder.Property(e => e.Email)
            .HasMaxLength(255);

        builder.Property(e => e.FullName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.AvatarUrl)
            .HasMaxLength(500);

        builder.Property(e => e.PasswordHash)
            .HasMaxLength(255);

        builder.Property(e => e.Role)
            .IsRequired()
            .HasDefaultValue(UserRole.Customer)
            .HasConversion<short>();

        builder.Property(e => e.IsVerified)
            .HasDefaultValue(false);

        builder.Property(e => e.IsActive)
            .HasDefaultValue(true);

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(e => e.ReferralCode)
            .IsRequired()
            .HasMaxLength(8);

        // ───────────────────────── Indexes ─────────────────────────

        builder.HasIndex(e => e.PhoneNumber)
            .HasDatabaseName("idx_users_phone")
            .HasFilter("\"is_deleted\" = false");

        builder.HasIndex(e => e.Email)
            .HasDatabaseName("idx_users_email")
            .HasFilter("\"email\" IS NOT NULL AND \"is_deleted\" = false");

        builder.HasIndex(e => e.Role)
            .HasDatabaseName("idx_users_role")
            .HasFilter("\"is_deleted\" = false");

        builder.HasIndex(e => e.ReferralCode)
            .IsUnique()
            .HasDatabaseName("idx_users_referral_code")
            .HasFilter("\"is_deleted\" = false");

        builder.HasIndex(e => e.ReferredByUserId)
            .HasDatabaseName("idx_users_referred_by")
            .HasFilter("\"referred_by_user_id\" IS NOT NULL AND \"is_deleted\" = false");
    }
}
