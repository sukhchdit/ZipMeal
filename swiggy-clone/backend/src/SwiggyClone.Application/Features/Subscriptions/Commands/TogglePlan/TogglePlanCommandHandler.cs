using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Subscriptions.Commands.TogglePlan;

internal sealed class TogglePlanCommandHandler(IAppDbContext db)
    : IRequestHandler<TogglePlanCommand, Result>
{
    public async Task<Result> Handle(TogglePlanCommand request, CancellationToken ct)
    {
        var plan = await db.SubscriptionPlans.FirstOrDefaultAsync(p => p.Id == request.Id, ct);
        if (plan is null)
            return Result.Failure("PLAN_NOT_FOUND", "Subscription plan not found.");

        plan.IsActive = request.IsActive;
        plan.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
