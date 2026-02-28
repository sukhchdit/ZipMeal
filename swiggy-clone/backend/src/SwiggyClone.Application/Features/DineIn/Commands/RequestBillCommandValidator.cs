using FluentValidation;

namespace SwiggyClone.Application.Features.DineIn.Commands;

public sealed class RequestBillCommandValidator : AbstractValidator<RequestBillCommand>
{
    public RequestBillCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.SessionId).NotEmpty();
    }
}
