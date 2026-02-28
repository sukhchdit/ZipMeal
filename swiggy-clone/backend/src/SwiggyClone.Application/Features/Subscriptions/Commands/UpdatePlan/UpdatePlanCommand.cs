using MediatR;
using SwiggyClone.Application.Features.Subscriptions.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Subscriptions.Commands.UpdatePlan;

public sealed record UpdatePlanCommand(
    Guid Id,
    string Name,
    string? Description,
    int PricePaise,
    int DurationDays,
    bool FreeDelivery,
    int ExtraDiscountPercent,
    bool NoSurgeFee) : IRequest<Result<AdminSubscriptionPlanDto>>;
