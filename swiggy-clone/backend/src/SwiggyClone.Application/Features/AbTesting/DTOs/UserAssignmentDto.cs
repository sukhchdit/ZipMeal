namespace SwiggyClone.Application.Features.AbTesting.DTOs;

public sealed record UserAssignmentDto(
    string ExperimentKey,
    string VariantKey,
    string? ConfigJson,
    DateTimeOffset AssignedAt);
