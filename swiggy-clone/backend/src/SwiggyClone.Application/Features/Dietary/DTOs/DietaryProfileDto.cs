namespace SwiggyClone.Application.Features.Dietary.DTOs;

public sealed record DietaryProfileDto(
    Guid Id,
    Guid UserId,
    short[]? AllergenAlerts,
    short[]? DietaryPreferences,
    short? MaxSpiceLevel);
