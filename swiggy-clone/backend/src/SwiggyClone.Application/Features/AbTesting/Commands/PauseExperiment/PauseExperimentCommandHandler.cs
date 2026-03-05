using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.AbTesting.Commands.PauseExperiment;

internal sealed class PauseExperimentCommandHandler(IAppDbContext db)
    : IRequestHandler<PauseExperimentCommand, Result>
{
    public async Task<Result> Handle(PauseExperimentCommand request, CancellationToken ct)
    {
        var experiment = await db.Experiments
            .FirstOrDefaultAsync(e => e.Id == request.ExperimentId, ct);

        if (experiment is null)
            return Result.Failure("EXPERIMENT_NOT_FOUND", "Experiment not found.");

        if (experiment.Status != ExperimentStatus.Active)
            return Result.Failure("EXPERIMENT_INVALID_STATUS_TRANSITION", "Only active experiments can be paused.");

        experiment.Status = ExperimentStatus.Paused;
        experiment.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
