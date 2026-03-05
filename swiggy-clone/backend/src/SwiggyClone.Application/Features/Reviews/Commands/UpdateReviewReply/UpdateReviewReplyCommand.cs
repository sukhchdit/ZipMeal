using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Reviews.Commands.UpdateReviewReply;

public sealed record UpdateReviewReplyCommand(
    Guid ReviewId,
    Guid OwnerId,
    string ReplyText) : IRequest<Result>;
