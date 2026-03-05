using MediatR;
using SwiggyClone.Application.Features.AbTesting.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.AbTesting.Commands.CreateExperiment;

public sealed record CreateExperimentVariantInput(
    string Key,
    string Name,
    int AllocationPercent,
    string? ConfigJson,
    bool IsControl);

public sealed record CreateExperimentCommand(
    Guid CreatedByUserId,
    string Key,
    string Name,
    string? Description,
    string? TargetAudience,
    DateTimeOffset? StartDate,
    DateTimeOffset? EndDate,
    string? GoalDescription,
    IReadOnlyList<CreateExperimentVariantInput> Variants) : IRequest<Result<ExperimentDto>>;
