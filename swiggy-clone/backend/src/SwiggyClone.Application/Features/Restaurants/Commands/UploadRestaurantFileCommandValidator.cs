using FluentValidation;

namespace SwiggyClone.Application.Features.Restaurants.Commands;

public sealed class UploadRestaurantFileCommandValidator : AbstractValidator<UploadRestaurantFileCommand>
{
    public UploadRestaurantFileCommandValidator()
    {
        RuleFor(x => x.FileType)
            .NotEmpty().WithMessage("File type is required.");

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("File name is required.");
    }
}
