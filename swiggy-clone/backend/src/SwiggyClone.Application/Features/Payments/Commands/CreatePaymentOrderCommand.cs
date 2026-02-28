using MediatR;
using SwiggyClone.Application.Features.Payments.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Payments.Commands;

public sealed record CreatePaymentOrderCommand(
    Guid UserId,
    Guid OrderId,
    int PaymentMethod) : IRequest<Result<CreatePaymentOrderResponseDto>>;
