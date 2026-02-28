using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SwiggyClone.Domain.Entities;

namespace SwiggyClone.Infrastructure.Persistence.Configurations;

internal sealed class DineInSessionMemberConfiguration : IEntityTypeConfiguration<DineInSessionMember>
{
    public void Configure(EntityTypeBuilder<DineInSessionMember> builder)
    {
        builder.ToTable("dine_in_session_members");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        // ───────────────────────── Scalar Properties ─────────────────────────

        builder.Property(e => e.Role)
            .HasDefaultValue((short)1);

        builder.Property(e => e.JoinedAt)
            .IsRequired();

        // ───────────────────────── Relationships ─────────────────────────

        builder.HasOne(e => e.Session)
            .WithMany(s => s.Members)
            .HasForeignKey(e => e.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
