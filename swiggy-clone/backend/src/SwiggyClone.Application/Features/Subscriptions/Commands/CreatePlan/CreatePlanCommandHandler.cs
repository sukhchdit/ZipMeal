using MediatR;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Subscriptions.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Subscriptions.Commands.CreatePlan;

internal sealed class CreatePlanCommandHandler(IAppDbContext db)
    : IRequestHandler<CreatePlanCommand, Result<AdminSubscriptionPlanDto>>
{
    public async Task<Result<AdminSubscriptionPlanDto>> Handle(CreatePlanCommand request, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        var plan = new SubscriptionPlan
        {
            Id = Guid.CreateVersion7(),
            Name = request.Name,
            Description = request.Description,
            PricePaise = request.PricePaise,
            DurationDays = request.DurationDays,
            FreeDelivery = request.FreeDelivery,
            ExtraDiscountPercent = request.ExtraDiscountPercent,
            NoSurgeFee = request.NoSurgeFee,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now,
        };

        db.SubscriptionPlans.Add(plan);
        await db.SaveChangesAsync(ct);

        var dto = new AdminSubscriptionPlanDto(
            plan.Id, plan.Name, plan.Description, plan.PricePaise, plan.DurationDays,
            plan.FreeDelivery, plan.ExtraDiscountPercent, plan.NoSurgeFee, plan.IsActive,
            plan.CreatedAt, plan.UpdatedAt);

        return Result<AdminSubscriptionPlanDto>.Success(dto);
    }
}
