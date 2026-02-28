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
