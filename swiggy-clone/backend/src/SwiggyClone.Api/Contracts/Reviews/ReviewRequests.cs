using SwiggyClone.Domain.Enums;

namespace SwiggyClone.Api.Contracts.Reviews;

public sealed record SubmitReviewRequest(
    Guid OrderId,
    short Rating,
    string? ReviewText,
    short? DeliveryRating,
    bool IsAnonymous,
    List<string> PhotoUrls);

public sealed record ReplyToReviewRequest(string ReplyText);

public sealed record ToggleReviewVisibilityRequest(bool IsVisible);

public sealed record VoteReviewRequest(bool IsHelpful);

public sealed record ReportReviewRequest(ReviewReportReason Reason, string? Description);

public sealed record ResolveReportRequest(ReviewReportStatus Status, string? AdminNotes);
