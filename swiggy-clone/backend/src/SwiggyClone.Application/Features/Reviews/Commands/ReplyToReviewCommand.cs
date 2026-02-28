using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Reviews.Commands;

public sealed record ReplyToReviewCommand(
    Guid ReviewId,
    Guid OwnerId,
    string ReplyText) : IRequest<Result>;
