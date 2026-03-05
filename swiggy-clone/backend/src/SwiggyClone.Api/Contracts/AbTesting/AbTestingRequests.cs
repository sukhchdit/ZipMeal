namespace SwiggyClone.Api.Contracts.AbTesting;

public sealed record CreateExperimentVariantRequest(
    string Key,
    string Name,
    int AllocationPercent,
    string? ConfigJson,
    bool IsControl);

public sealed record CreateExperimentRequest(
    string Key,
    string Name,
    string? Description,
    string? TargetAudience,
    DateTimeOffset? StartDate,
    DateTimeOffset? EndDate,
    string? GoalDescription,
    IReadOnlyList<CreateExperimentVariantRequest> Variants);

public sealed record UpdateExperimentRequest(
    string Name,
    string? Description,
    string? TargetAudience,
    DateTimeOffset? StartDate,
    DateTimeOffset? EndDate,
    string? GoalDescription);

public sealed record RecordExposureRequest(
    string ExperimentKey,
    string? Context);

public sealed record RecordConversionRequest(
    string ExperimentKey,
    string GoalKey,
    decimal? Value);
