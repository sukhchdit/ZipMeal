using MediatR;
using SwiggyClone.Application.Features.AbTesting.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.AbTesting.Queries.GetExperiments;

public sealed record GetExperimentsQuery(
    int? Status,
    int Page = 1,
    int PageSize = 20) : IRequest<Result<PagedExperimentsResult>>;

public sealed record PagedExperimentsResult(
    IReadOnlyList<ExperimentDto> Items,
    int TotalCount,
    int Page,
    int PageSize);
