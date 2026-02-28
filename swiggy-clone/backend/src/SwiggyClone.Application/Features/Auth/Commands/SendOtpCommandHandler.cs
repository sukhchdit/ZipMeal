using MediatR;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Auth.Commands;

internal sealed class SendOtpCommandHandler(IOtpService otpService)
    : IRequestHandler<SendOtpCommand, Result>
{
    public async Task<Result> Handle(SendOtpCommand request, CancellationToken ct)
    {
        await otpService.GenerateAndSendOtpAsync(request.PhoneNumber, ct);
        return Result.Success();
    }
}
