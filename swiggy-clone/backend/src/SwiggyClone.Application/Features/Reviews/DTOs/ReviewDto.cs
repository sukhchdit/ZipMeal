namespace SwiggyClone.Application.Features.Reviews.DTOs;

public sealed record ReviewDto(
    Guid Id,
    Guid OrderId,
    Guid UserId,
    string? ReviewerName,
    string? ReviewerAvatarUrl,
    Guid RestaurantId,
    short Rating,
    string? ReviewText,
    short? DeliveryRating,
    bool IsAnonymous,
    bool IsVisible,
    string? RestaurantReply,
    DateTimeOffset? RepliedAt,
    DateTimeOffset CreatedAt,
    List<ReviewPhotoDto> Photos);
