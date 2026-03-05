namespace SwiggyClone.Application.Features.AbTesting.DTOs;

public sealed record ExperimentStatsDto(
    Guid ExperimentId,
    IReadOnlyList<VariantStatsDto> Variants,
    DateTimeOffset ComputedAt);
