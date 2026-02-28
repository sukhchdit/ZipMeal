using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Coupons.Commands;

public sealed record ToggleCouponCommand(Guid Id, bool IsActive) : IRequest<Result>;
