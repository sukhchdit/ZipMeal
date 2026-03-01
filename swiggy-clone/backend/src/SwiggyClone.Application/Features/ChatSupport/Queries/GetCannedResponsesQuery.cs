using MediatR;
using SwiggyClone.Application.Features.ChatSupport.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.ChatSupport.Queries;

public sealed record GetCannedResponsesQuery(
    int? Category) : IRequest<Result<IReadOnlyList<CannedResponseDto>>>;
