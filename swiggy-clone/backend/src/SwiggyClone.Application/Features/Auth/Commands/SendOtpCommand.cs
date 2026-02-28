using MediatR;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Auth.Commands;

public sealed record SendOtpCommand(string PhoneNumber) : IRequest<Result>;
