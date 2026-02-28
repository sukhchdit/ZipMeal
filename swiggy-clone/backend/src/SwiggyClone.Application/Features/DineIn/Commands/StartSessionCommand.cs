using MediatR;
using SwiggyClone.Application.Features.DineIn.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.DineIn.Commands;

public sealed record StartSessionCommand(
    Guid UserId,
    string QrCodeData,
    int GuestCount) : IRequest<Result<DineInSessionDto>>;
