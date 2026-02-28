using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Reviews.Commands;

public sealed record ToggleReviewVisibilityCommand(
    Guid ReviewId,
    bool IsVisible) : IRequest<Result>;
