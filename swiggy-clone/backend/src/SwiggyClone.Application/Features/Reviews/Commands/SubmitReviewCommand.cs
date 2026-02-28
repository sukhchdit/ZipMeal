using MediatR;
using SwiggyClone.Application.Features.Reviews.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Reviews.Commands;

public sealed record SubmitReviewCommand(
    Guid UserId,
    Guid OrderId,
    short Rating,
    string? ReviewText,
    short? DeliveryRating,
    bool IsAnonymous,
    List<string> PhotoUrls) : IRequest<Result<ReviewDto>>;
