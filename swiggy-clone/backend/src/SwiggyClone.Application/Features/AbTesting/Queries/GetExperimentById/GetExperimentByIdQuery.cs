using MediatR;
using SwiggyClone.Application.Features.AbTesting.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.AbTesting.Queries.GetExperimentById;

public sealed record GetExperimentByIdQuery(Guid ExperimentId) : IRequest<Result<ExperimentDto>>;
