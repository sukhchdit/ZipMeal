using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.AbTesting.Commands.CompleteExperiment;

internal sealed class CompleteExperimentCommandHandler(IAppDbContext db)
    : IRequestHandler<CompleteExperimentCommand, Result>
{
    public async Task<Result> Handle(CompleteExperimentCommand request, CancellationToken ct)
    {
        var experiment = await db.Experiments
            .FirstOrDefaultAsync(e => e.Id == request.ExperimentId, ct);

        if (experiment is null)
            return Result.Failure("EXPERIMENT_NOT_FOUND", "Experiment not found.");

        if (experiment.Status == ExperimentStatus.Archived)
            return Result.Failure("EXPERIMENT_INVALID_STATUS_TRANSITION", "Archived experiments cannot be completed.");

        experiment.Status = ExperimentStatus.Completed;
        experiment.EndDate ??= DateTimeOffset.UtcNow;
        experiment.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
