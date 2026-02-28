using FluentValidation;

namespace SwiggyClone.Application.Features.DineIn.Commands;

public sealed class UpdateTableCommandValidator : AbstractValidator<UpdateTableCommand>
{
    public UpdateTableCommandValidator()
    {
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.RestaurantId).NotEmpty();
        RuleFor(x => x.TableId).NotEmpty();
        RuleFor(x => x.TableNumber).MaximumLength(20)
            .When(x => x.TableNumber is not null);
        RuleFor(x => x.Capacity).InclusiveBetween(1, 20)
            .When(x => x.Capacity.HasValue);
        RuleFor(x => x.FloorSection).MaximumLength(50)
            .When(x => x.FloorSection is not null);
        RuleFor(x => x.Status).IsInEnum()
            .When(x => x.Status.HasValue);
    }
}
