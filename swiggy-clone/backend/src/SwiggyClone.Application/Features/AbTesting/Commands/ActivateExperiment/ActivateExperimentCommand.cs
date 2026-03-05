using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.AbTesting.Commands.ActivateExperiment;

public sealed record ActivateExperimentCommand(Guid ExperimentId) : IRequest<Result>;
