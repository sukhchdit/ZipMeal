using FluentValidation;

namespace SwiggyClone.Application.Features.DineIn.Commands;

public sealed class CreateTableCommandValidator : AbstractValidator<CreateTableCommand>
{
    public CreateTableCommandValidator()
    {
        RuleFor(x => x.OwnerId).NotEmpty();
        RuleFor(x => x.RestaurantId).NotEmpty();
        RuleFor(x => x.TableNumber).NotEmpty().MaximumLength(20)
            .WithMessage("Table number is required and must be at most 20 characters.");
        RuleFor(x => x.Capacity).InclusiveBetween(1, 20)
            .WithMessage("Capacity must be between 1 and 20.");
        RuleFor(x => x.FloorSection).MaximumLength(50);
    }
}
