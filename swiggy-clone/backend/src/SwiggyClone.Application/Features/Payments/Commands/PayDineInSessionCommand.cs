using MediatR;
using SwiggyClone.Application.Features.Payments.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Payments.Commands;

public sealed record PayDineInSessionCommand(
    Guid UserId,
    Guid SessionId,
    int PaymentMethod) : IRequest<Result<CreatePaymentOrderResponseDto>>;
