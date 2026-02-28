using MediatR;
using SwiggyClone.Application.Features.Payments.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Payments.Commands;

public sealed record InitiateRefundCommand(
    Guid OrderId,
    string? Reason) : IRequest<Result<PaymentDto>>;
