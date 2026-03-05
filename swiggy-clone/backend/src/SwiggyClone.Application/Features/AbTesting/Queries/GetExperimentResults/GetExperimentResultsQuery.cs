using MediatR;
using SwiggyClone.Application.Features.AbTesting.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.AbTesting.Queries.GetExperimentResults;

public sealed record GetExperimentResultsQuery(Guid ExperimentId) : IRequest<Result<ExperimentStatsDto>>;
