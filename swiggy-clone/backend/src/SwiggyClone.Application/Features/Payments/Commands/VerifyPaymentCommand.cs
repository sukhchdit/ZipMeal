using MediatR;
using SwiggyClone.Application.Features.Payments.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Payments.Commands;

public sealed record VerifyPaymentCommand(
    Guid UserId,
    string GatewayOrderId,
    string GatewayPaymentId,
    string GatewaySignature) : IRequest<Result<PaymentDto>>;
