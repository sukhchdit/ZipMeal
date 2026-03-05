namespace SwiggyClone.Application.Features.AbTesting.DTOs;

public sealed record ExperimentVariantDto(
    Guid Id,
    string Key,
    string Name,
    int AllocationPercent,
    string? ConfigJson,
    bool IsControl);
