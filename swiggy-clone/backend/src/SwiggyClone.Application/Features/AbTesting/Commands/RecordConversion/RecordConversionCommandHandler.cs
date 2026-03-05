using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.AbTesting.Commands.RecordConversion;

internal sealed class RecordConversionCommandHandler(IAppDbContext db)
    : IRequestHandler<RecordConversionCommand, Result>
{
    public async Task<Result> Handle(RecordConversionCommand request, CancellationToken ct)
    {
        var assignment = await db.UserVariantAssignments.AsNoTracking()
            .FirstOrDefaultAsync(a =>
                a.UserId == request.UserId
                && a.Experiment.Key == request.ExperimentKey, ct);

        if (assignment is null)
            return Result.Success(); // Graceful no-op if not assigned

        var conversion = new ConversionEvent
        {
            Id = Guid.CreateVersion7(),
            AssignmentId = assignment.Id,
            GoalKey = request.GoalKey,
            Value = request.Value,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        db.ConversionEvents.Add(conversion);
        await db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
