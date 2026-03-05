using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Reviews.Commands.VoteReview;

public sealed record VoteReviewCommand(
    Guid ReviewId,
    Guid UserId,
    bool IsHelpful) : IRequest<Result>;
