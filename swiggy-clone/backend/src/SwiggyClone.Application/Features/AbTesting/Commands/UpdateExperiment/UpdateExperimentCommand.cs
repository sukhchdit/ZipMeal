using MediatR;
using SwiggyClone.Application.Features.AbTesting.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.AbTesting.Commands.UpdateExperiment;

public sealed record UpdateExperimentCommand(
    Guid ExperimentId,
    string Name,
    string? Description,
    string? TargetAudience,
    DateTimeOffset? StartDate,
    DateTimeOffset? EndDate,
    string? GoalDescription) : IRequest<Result<ExperimentDto>>;
