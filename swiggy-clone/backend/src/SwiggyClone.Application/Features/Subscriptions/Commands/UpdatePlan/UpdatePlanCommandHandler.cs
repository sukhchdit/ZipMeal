using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Subscriptions.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Subscriptions.Commands.UpdatePlan;

internal sealed class UpdatePlanCommandHandler(IAppDbContext db)
    : IRequestHandler<UpdatePlanCommand, Result<AdminSubscriptionPlanDto>>
{
    public async Task<Result<AdminSubscriptionPlanDto>> Handle(UpdatePlanCommand request, CancellationToken ct)
    {
        var plan = await db.SubscriptionPlans.FirstOrDefaultAsync(p => p.Id == request.Id, ct);
        if (plan is null)
            return Result<AdminSubscriptionPlanDto>.Failure("PLAN_NOT_FOUND", "Subscription plan not found.");

        plan.Name = request.Name;
        plan.Description = request.Description;
        plan.PricePaise = request.PricePaise;
        plan.DurationDays = request.DurationDays;
        plan.FreeDelivery = request.FreeDelivery;
        plan.ExtraDiscountPercent = request.ExtraDiscountPercent;
        plan.NoSurgeFee = request.NoSurgeFee;
        plan.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);

        var dto = new AdminSubscriptionPlanDto(
            plan.Id, plan.Name, plan.Description, plan.PricePaise, plan.DurationDays,
            plan.FreeDelivery, plan.ExtraDiscountPercent, plan.NoSurgeFee, plan.IsActive,
            plan.CreatedAt, plan.UpdatedAt);

        return Result<AdminSubscriptionPlanDto>.Success(dto);
    }
}
