using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.AbTesting.Commands.RecordExposure;

internal sealed class RecordExposureCommandHandler(IAppDbContext db)
    : IRequestHandler<RecordExposureCommand, Result>
{
    public async Task<Result> Handle(RecordExposureCommand request, CancellationToken ct)
    {
        // Find the user's assignment for this experiment
        var assignment = await db.UserVariantAssignments.AsNoTracking()
            .FirstOrDefaultAsync(a =>
                a.UserId == request.UserId
                && a.Experiment.Key == request.ExperimentKey, ct);

        if (assignment is null)
            return Result.Success(); // Graceful no-op if not assigned

        var exposure = new ExposureEvent
        {
            Id = Guid.CreateVersion7(),
            AssignmentId = assignment.Id,
            Context = request.Context,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        db.ExposureEvents.Add(exposure);
        await db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
