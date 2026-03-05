using MediatR;
using SwiggyClone.Application.Features.Disputes.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Disputes.Commands.CreateDispute;

public sealed record CreateDisputeCommand(
    Guid UserId,
    Guid OrderId,
    int IssueType,
    string Description) : IRequest<Result<DisputeDto>>;
