using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.AbTesting.Commands.ActivateExperiment;

internal sealed class ActivateExperimentCommandHandler(IAppDbContext db)
    : IRequestHandler<ActivateExperimentCommand, Result>
{
    public async Task<Result> Handle(ActivateExperimentCommand request, CancellationToken ct)
    {
        var experiment = await db.Experiments
            .FirstOrDefaultAsync(e => e.Id == request.ExperimentId, ct);

        if (experiment is null)
            return Result.Failure("EXPERIMENT_NOT_FOUND", "Experiment not found.");

        if (experiment.Status != ExperimentStatus.Draft && experiment.Status != ExperimentStatus.Paused)
            return Result.Failure("EXPERIMENT_INVALID_STATUS_TRANSITION", "Only draft or paused experiments can be activated.");

        experiment.Status = ExperimentStatus.Active;
        experiment.StartDate ??= DateTimeOffset.UtcNow;
        experiment.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
