using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Domain.Enums;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Subscriptions.Commands.CancelSubscription;

internal sealed class CancelSubscriptionCommandHandler(IAppDbContext db)
    : IRequestHandler<CancelSubscriptionCommand, Result>
{
    public async Task<Result> Handle(CancelSubscriptionCommand request, CancellationToken ct)
    {
        var subscription = await db.UserSubscriptions
            .FirstOrDefaultAsync(s => s.UserId == request.UserId
                && s.Status == SubscriptionStatus.Active
                && s.EndDate > DateTimeOffset.UtcNow, ct);

        if (subscription is null)
            return Result.Failure("NO_ACTIVE_SUBSCRIPTION", "You do not have an active subscription to cancel.");

        subscription.Status = SubscriptionStatus.Cancelled;

        await db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
