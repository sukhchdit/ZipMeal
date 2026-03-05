using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.AbTesting.Commands.RecordConversion;

public sealed record RecordConversionCommand(
    Guid UserId,
    string ExperimentKey,
    string GoalKey,
    decimal? Value) : IRequest<Result>;
