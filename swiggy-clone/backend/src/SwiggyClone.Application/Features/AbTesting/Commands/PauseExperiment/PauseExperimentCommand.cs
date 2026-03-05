using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.AbTesting.Commands.PauseExperiment;

public sealed record PauseExperimentCommand(Guid ExperimentId) : IRequest<Result>;
