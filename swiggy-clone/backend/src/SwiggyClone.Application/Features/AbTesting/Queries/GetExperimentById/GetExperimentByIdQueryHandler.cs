using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.AbTesting.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.AbTesting.Queries.GetExperimentById;

internal sealed class GetExperimentByIdQueryHandler(IAppDbContext db)
    : IRequestHandler<GetExperimentByIdQuery, Result<ExperimentDto>>
{
    public async Task<Result<ExperimentDto>> Handle(GetExperimentByIdQuery request, CancellationToken ct)
    {
        var experiment = await db.Experiments.AsNoTracking()
            .Include(e => e.Variants)
            .FirstOrDefaultAsync(e => e.Id == request.ExperimentId, ct);

        if (experiment is null)
            return Result<ExperimentDto>.Failure("EXPERIMENT_NOT_FOUND", "Experiment not found.");

        var variantDtos = experiment.Variants
            .Select(v => new ExperimentVariantDto(
                v.Id, v.Key, v.Name, v.AllocationPercent, v.ConfigJson, v.IsControl))
            .ToList();

        return Result<ExperimentDto>.Success(new ExperimentDto(
            experiment.Id, experiment.Key, experiment.Name, experiment.Description,
            (int)experiment.Status, experiment.TargetAudience,
            experiment.StartDate, experiment.EndDate, experiment.GoalDescription,
            experiment.CreatedByUserId, variantDtos,
            experiment.CreatedAt, experiment.UpdatedAt));
    }
}
