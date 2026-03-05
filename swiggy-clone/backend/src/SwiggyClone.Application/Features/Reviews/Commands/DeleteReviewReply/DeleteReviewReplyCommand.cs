using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Reviews.Commands.DeleteReviewReply;

public sealed record DeleteReviewReplyCommand(
    Guid ReviewId,
    Guid OwnerId) : IRequest<Result>;
