using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.AbTesting.Commands.RecordExposure;

public sealed record RecordExposureCommand(
    Guid UserId,
    string ExperimentKey,
    string? Context) : IRequest<Result>;
