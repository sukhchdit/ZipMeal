using SwiggyClone.Domain.Common;

namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Stores a user's allergen alerts, dietary preferences, and maximum spice tolerance.
/// One-to-one relationship with <see cref="User"/> via unique UserId index.
/// </summary>
public sealed class UserDietaryProfile : BaseEntity
{
    public Guid UserId { get; set; }

    /// <summary>
    /// Allergen codes the user wants to be alerted about. PostgreSQL smallint[].
    /// </summary>
    public short[]? AllergenAlerts { get; set; }

    /// <summary>
    /// Dietary preference codes the user follows. PostgreSQL smallint[].
    /// </summary>
    public short[]? DietaryPreferences { get; set; }

    /// <summary>
    /// Maximum spice level the user can tolerate (0–4). Null means no preference.
    /// </summary>
    public short? MaxSpiceLevel { get; set; }

    // ───────────────────────── Navigation Properties ─────────────────────────

    public User User { get; set; } = null!;

    // ───────────────────────── Factory ─────────────────────────

    public static UserDietaryProfile Create(
        Guid userId, short[]? alerts, short[]? prefs, short? maxSpice)
    {
        var now = DateTimeOffset.UtcNow;
        return new UserDietaryProfile
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            AllergenAlerts = alerts,
            DietaryPreferences = prefs,
            MaxSpiceLevel = maxSpice,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void Update(short[]? alerts, short[]? prefs, short? maxSpice)
    {
        AllergenAlerts = alerts;
        DietaryPreferences = prefs;
        MaxSpiceLevel = maxSpice;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
