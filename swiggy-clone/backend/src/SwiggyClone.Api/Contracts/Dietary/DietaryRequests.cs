namespace SwiggyClone.Api.Contracts.Dietary;

public sealed record SaveDietaryProfileRequest(
    short[]? AllergenAlerts,
    short[]? DietaryPreferences,
    short? MaxSpiceLevel);
