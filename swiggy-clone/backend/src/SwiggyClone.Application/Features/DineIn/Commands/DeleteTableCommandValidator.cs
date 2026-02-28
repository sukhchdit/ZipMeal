using FluentValidation;

namespace SwiggyClone.Application.Features.DineIn.Commands;

public sealed class DeleteTableCommandValidator : AbstractValidator<DeleteTableCommand>
{
    public DeleteTableCommandValidator()
    {
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.RestaurantId).NotEmpty();
        RuleFor(x => x.TableId).NotEmpty();
    }
}
