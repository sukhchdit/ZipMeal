using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class UserDietaryProfileConfiguration : IEntityTypeConfiguration<UserDietaryProfile>
{
    public void Configure(EntityTypeBuilder<UserDietaryProfile> builder)
    {
        builder.ToTable("user_dietary_profiles");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.AllergenAlerts)
            .HasColumnType("smallint[]");

        builder.Property(e => e.DietaryPreferences)
            .HasColumnType("smallint[]");

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // ───────────────────────── Relationships ─────────────────────────

        builder.HasOne(e => e.User)
            .WithOne()
            .HasForeignKey<UserDietaryProfile>(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ───────────────────────── Indexes ─────────────────────────

        builder.HasIndex(e => e.UserId)
            .IsUnique()
            .HasDatabaseName("idx_user_dietary_profiles_user_id");
    }
}
