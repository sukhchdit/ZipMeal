using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.AbTesting.Commands.CompleteExperiment;

public sealed record CompleteExperimentCommand(Guid ExperimentId) : IRequest<Result>;
