using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Application.Features.Reviews.DTOs;

public sealed record ReviewReportDto(
    Guid Id,
    Guid ReviewId,
    string? ReviewText,
    short ReviewRating,
    string ReviewerName,
    string ReporterName,
    ReviewReportReason Reason,
    string? Description,
    ReviewReportStatus Status,
    string? AdminNotes,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ResolvedAt);
