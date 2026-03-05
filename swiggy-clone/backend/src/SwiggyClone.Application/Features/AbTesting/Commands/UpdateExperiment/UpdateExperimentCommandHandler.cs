using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.AbTesting.DTOs;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.AbTesting.Commands.UpdateExperiment;

internal sealed class UpdateExperimentCommandHandler(IAppDbContext db)
    : IRequestHandler<UpdateExperimentCommand, Result<ExperimentDto>>
{
    public async Task<Result<ExperimentDto>> Handle(UpdateExperimentCommand request, CancellationToken ct)
    {
        var experiment = await db.Experiments
            .Include(e => e.Variants)
            .FirstOrDefaultAsync(e => e.Id == request.ExperimentId, ct);

        if (experiment is null)
            return Result<ExperimentDto>.Failure("EXPERIMENT_NOT_FOUND", "Experiment not found.");

        if (experiment.Status != ExperimentStatus.Draft)
            return Result<ExperimentDto>.Failure("EXPERIMENT_INVALID_STATUS_TRANSITION", "Only draft experiments can be updated.");

        experiment.Name = request.Name;
        experiment.Description = request.Description;
        experiment.TargetAudience = request.TargetAudience;
        experiment.StartDate = request.StartDate;
        experiment.EndDate = request.EndDate;
        experiment.GoalDescription = request.GoalDescription;
        experiment.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);

        var variantDtos = experiment.Variants.Select(v => new ExperimentVariantDto(
            v.Id, v.Key, v.Name, v.AllocationPercent, v.ConfigJson, v.IsControl)).ToList();

        return Result<ExperimentDto>.Success(new ExperimentDto(
            experiment.Id, experiment.Key, experiment.Name, experiment.Description,
            (int)experiment.Status, experiment.TargetAudience,
            experiment.StartDate, experiment.EndDate, experiment.GoalDescription,
            experiment.CreatedByUserId, variantDtos,
            experiment.CreatedAt, experiment.UpdatedAt));
    }
}
