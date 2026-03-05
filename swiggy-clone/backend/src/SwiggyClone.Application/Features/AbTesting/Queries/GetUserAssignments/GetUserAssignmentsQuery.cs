using MediatR;
using SwiggyClone.Application.Features.AbTesting.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.AbTesting.Queries.GetUserAssignments;

public sealed record GetUserAssignmentsQuery(Guid UserId) : IRequest<Result<IReadOnlyList<UserAssignmentDto>>>;
