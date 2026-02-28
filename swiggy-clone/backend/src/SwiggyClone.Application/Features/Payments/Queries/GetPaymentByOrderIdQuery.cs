using MediatR;
using SwiggyClone.Application.Features.Payments.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Payments.Queries;

public sealed record GetPaymentByOrderIdQuery(
    Guid UserId,
    Guid OrderId) : IRequest<Result<PaymentDto>>;
