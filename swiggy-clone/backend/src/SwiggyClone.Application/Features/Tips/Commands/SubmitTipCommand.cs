using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Tips.Commands;

public sealed record SubmitTipCommand(Guid UserId, Guid OrderId, int AmountPaise) : IRequest<Result>;
