using MediatR;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Reviews.Commands.ReportReview;

public sealed record ReportReviewCommand(
    Guid ReviewId,
    Guid UserId,
    ReviewReportReason Reason,
    string? Description) : IRequest<Result>;
