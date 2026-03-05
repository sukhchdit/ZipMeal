using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Reviews.Commands.RemoveReviewVote;

public sealed record RemoveReviewVoteCommand(
    Guid ReviewId,
    Guid UserId) : IRequest<Result>;
