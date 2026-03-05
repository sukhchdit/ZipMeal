using System.Security.Cryptography;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.AbTesting.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.AbTesting.Queries.GetUserAssignments;

internal sealed class GetUserAssignmentsQueryHandler(
    IAppDbContext db,
    ILogger<GetUserAssignmentsQueryHandler> logger)
    : IRequestHandler<GetUserAssignmentsQuery, Result<IReadOnlyList<UserAssignmentDto>>>
{
    public async Task<Result<IReadOnlyList<UserAssignmentDto>>> Handle(
        GetUserAssignmentsQuery request, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;

        // 1. Load active experiments with variants
        var activeExperiments = await db.Experiments.AsNoTracking()
            .Include(e => e.Variants)
            .Where(e => e.Status == ExperimentStatus.Active
                && (e.EndDate == null || e.EndDate > now))
            .ToListAsync(ct);

        if (activeExperiments.Count == 0)
            return Result<IReadOnlyList<UserAssignmentDto>>.Success([]);

        // 2. Load existing assignments for user
        var experimentIds = activeExperiments.Select(e => e.Id).ToList();
        var existingAssignments = await db.UserVariantAssignments.AsNoTracking()
            .Include(a => a.Variant)
            .Include(a => a.Experiment)
            .Where(a => a.UserId == request.UserId && experimentIds.Contains(a.ExperimentId))
            .ToDictionaryAsync(a => a.ExperimentId, ct);

        // 3. Assign unassigned experiments
        var newAssignments = new List<UserVariantAssignment>();

        foreach (var experiment in activeExperiments)
        {
            if (existingAssignments.ContainsKey(experiment.Id))
                continue;

            var variant = AssignVariant(request.UserId, experiment);
            if (variant is null)
                continue;

            var assignment = new UserVariantAssignment
            {
                Id = Guid.CreateVersion7(),
                UserId = request.UserId,
                ExperimentId = experiment.Id,
                VariantId = variant.Id,
                AssignedAt = now,
            };

            newAssignments.Add(assignment);
        }

        // 4. Batch insert new assignments
        if (newAssignments.Count > 0)
        {
            try
            {
                db.UserVariantAssignments.AddRange(newAssignments);
                await db.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true)
            {
                // Race condition safety — another request already assigned
                logger.LogWarning(ex, "Duplicate assignment detected for user {UserId}, ignoring", request.UserId);
            }
        }

        // 5. Build result — re-query for consistency
        var allAssignments = await db.UserVariantAssignments.AsNoTracking()
            .Include(a => a.Experiment)
            .Include(a => a.Variant)
            .Where(a => a.UserId == request.UserId
                && a.Experiment.Status == ExperimentStatus.Active
                && (a.Experiment.EndDate == null || a.Experiment.EndDate > now))
            .Select(a => new UserAssignmentDto(
                a.Experiment.Key,
                a.Variant.Key,
                a.Variant.ConfigJson,
                a.AssignedAt))
            .ToListAsync(ct);

        return Result<IReadOnlyList<UserAssignmentDto>>.Success(allAssignments);
    }

    private static ExperimentVariant? AssignVariant(Guid userId, Experiment experiment)
    {
        if (experiment.Variants.Count == 0)
            return null;

        // SHA-256 deterministic hash
        var input = $"{userId}:{experiment.Key}";
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        var bucket = (int)(BitConverter.ToUInt64(hashBytes, 0) % 100);

        // Sort variants by key alphabetically, walk cumulative allocation
        var sortedVariants = experiment.Variants.OrderBy(v => v.Key).ToList();
        var cumulative = 0;

        foreach (var variant in sortedVariants)
        {
            cumulative += variant.AllocationPercent;
            if (bucket < cumulative)
                return variant;
        }

        // Fallback: return last variant
        return sortedVariants[^1];
    }
}
