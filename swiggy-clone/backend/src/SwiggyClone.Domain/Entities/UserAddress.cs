using SwiggyClone.Domain.Common;

namespace SwiggyClone.Domain.Entities;

/// <summary>
/// Represents a saved address for a user (e.g., Home, Work, Other).
/// Supports soft-delete so users can remove addresses without losing order history references.
/// </summary>
public class UserAddress : BaseEntity
{
    /// <summary>
    /// Foreign key to the owning <see cref="User"/>.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// User-defined label for quick identification (e.g., "Home", "Work", "Mom's Place").
    /// </summary>
    public string Label { get; set; } = default!;

    /// <summary>
    /// Primary street address line (house/flat number, street name).
    /// </summary>
    public string AddressLine1 { get; set; } = default!;

    /// <summary>
    /// Secondary address line for landmarks, floor, building name, etc.
    /// </summary>
    public string? AddressLine2 { get; set; }

    /// <summary>
    /// City name.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// State or province name.
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Postal/ZIP code.
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// Country name. Defaults to "India".
    /// </summary>
    public string? Country { get; set; } = "India";

    /// <summary>
    /// Geographic latitude coordinate for delivery routing and distance calculations.
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Geographic longitude coordinate for delivery routing and distance calculations.
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Indicates whether this is the user's default/primary address.
    /// Only one address per user should be marked as default at any given time.
    /// </summary>
    public bool IsDefault { get; set; }

    // ── Navigation Properties ──────────────────────────────────────────

    /// <summary>
    /// The user who owns this address.
    /// </summary>
    public User User { get; set; } = default!;
}
