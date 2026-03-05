using MediatR;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Reviews.Commands.ResolveReviewReport;

public sealed record ResolveReviewReportCommand(
    Guid ReportId,
    Guid AdminId,
    ReviewReportStatus Status,
    string? AdminNotes) : IRequest<Result>;
