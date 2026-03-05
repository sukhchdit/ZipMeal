using MediatR;
using SwiggyClone.Application.Features.Reviews.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Reviews.Queries.GetReviewReports;

public sealed record GetReviewReportsQuery(
    ReviewReportStatus? Status,
    int Page,
    int PageSize) : IRequest<Result<PagedResult<ReviewReportDto>>>;
