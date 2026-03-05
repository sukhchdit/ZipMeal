using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.AbTesting.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.AbTesting.Commands.CreateExperiment;

internal sealed class CreateExperimentCommandHandler(IAppDbContext db)
    : IRequestHandler<CreateExperimentCommand, Result<ExperimentDto>>
{
    public async Task<Result<ExperimentDto>> Handle(CreateExperimentCommand request, CancellationToken ct)
    {
        // 1. Check key uniqueness
        var keyExists = await db.Experiments.AsNoTracking()
            .AnyAsync(e => e.Key == request.Key, ct);

        if (keyExists)
            return Result<ExperimentDto>.Failure("EXPERIMENT_KEY_DUPLICATE", "An experiment with this key already exists.");

        var now = DateTimeOffset.UtcNow;

        // 2. Create experiment
        var experiment = new Experiment
        {
            Id = Guid.CreateVersion7(),
            Key = request.Key,
            Name = request.Name,
            Description = request.Description,
            Status = ExperimentStatus.Draft,
            TargetAudience = request.TargetAudience,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            GoalDescription = request.GoalDescription,
            CreatedByUserId = request.CreatedByUserId,
            CreatedAt = now,
            UpdatedAt = now,
        };

        // 3. Create variants
        var variantDtos = new List<ExperimentVariantDto>();
        foreach (var v in request.Variants)
        {
            var variant = new ExperimentVariant
            {
                Id = Guid.CreateVersion7(),
                ExperimentId = experiment.Id,
                Key = v.Key,
                Name = v.Name,
                AllocationPercent = v.AllocationPercent,
                ConfigJson = v.ConfigJson,
                IsControl = v.IsControl,
                CreatedAt = now,
                UpdatedAt = now,
            };
            db.ExperimentVariants.Add(variant);
            variantDtos.Add(new ExperimentVariantDto(
                variant.Id, variant.Key, variant.Name,
                variant.AllocationPercent, variant.ConfigJson, variant.IsControl));
        }

        db.Experiments.Add(experiment);
        await db.SaveChangesAsync(ct);

        // 4. Return DTO
        return Result<ExperimentDto>.Success(new ExperimentDto(
            experiment.Id, experiment.Key, experiment.Name, experiment.Description,
            (int)experiment.Status, experiment.TargetAudience,
            experiment.StartDate, experiment.EndDate, experiment.GoalDescription,
            experiment.CreatedByUserId, variantDtos,
            experiment.CreatedAt, experiment.UpdatedAt));
    }
}
