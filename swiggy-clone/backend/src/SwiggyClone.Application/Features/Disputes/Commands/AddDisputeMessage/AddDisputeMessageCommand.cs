using MediatR;
using SwiggyClone.Application.Features.Disputes.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Disputes.Commands.AddDisputeMessage;

public sealed record AddDisputeMessageCommand(
    Guid UserId,
    Guid DisputeId,
    string Content) : IRequest<Result<DisputeMessageDto>>;
