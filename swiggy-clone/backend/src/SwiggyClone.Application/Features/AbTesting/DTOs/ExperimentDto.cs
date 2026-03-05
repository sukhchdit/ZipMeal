namespace SwiggyClone.Application.Features.AbTesting.DTOs;

public sealed record ExperimentDto(
    Guid Id,
    string Key,
    string Name,
    string? Description,
    int Status,
    string? TargetAudience,
    DateTimeOffset? StartDate,
    DateTimeOffset? EndDate,
    string? GoalDescription,
    Guid CreatedByUserId,
    IReadOnlyList<ExperimentVariantDto> Variants,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
