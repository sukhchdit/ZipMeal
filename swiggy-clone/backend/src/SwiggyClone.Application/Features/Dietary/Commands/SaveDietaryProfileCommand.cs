using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Dietary.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Dietary.Commands;

public sealed record SaveDietaryProfileCommand(
    Guid UserId,
    short[]? AllergenAlerts,
    short[]? DietaryPreferences,
    short? MaxSpiceLevel) : IRequest<Result<DietaryProfileDto>>;

internal sealed class SaveDietaryProfileCommandHandler(IAppDbContext db)
    : IRequestHandler<SaveDietaryProfileCommand, Result<DietaryProfileDto>>
{
    public async Task<Result<DietaryProfileDto>> Handle(
        SaveDietaryProfileCommand request, CancellationToken ct)
    {
        var profile = await db.UserDietaryProfiles
            .FirstOrDefaultAsync(p => p.UserId == request.UserId, ct);

        if (profile is not null)
        {
            profile.Update(request.AllergenAlerts, request.DietaryPreferences, request.MaxSpiceLevel);
        }
        else
        {
            profile = UserDietaryProfile.Create(
                request.UserId, request.AllergenAlerts, request.DietaryPreferences, request.MaxSpiceLevel);
            db.UserDietaryProfiles.Add(profile);
        }

        await db.SaveChangesAsync(ct);

        var dto = new DietaryProfileDto(
            profile.Id, profile.UserId,
            profile.AllergenAlerts, profile.DietaryPreferences, profile.MaxSpiceLevel);

        return Result<DietaryProfileDto>.Success(dto);
    }
}

public sealed class SaveDietaryProfileCommandValidator : AbstractValidator<SaveDietaryProfileCommand>
{
    public SaveDietaryProfileCommandValidator()
    {
        RuleFor(x => x.MaxSpiceLevel)
            .InclusiveBetween((short)0, (short)4).WithMessage("Invalid spice level.")
            .When(x => x.MaxSpiceLevel.HasValue);

        RuleForEach(x => x.AllergenAlerts)
            .InclusiveBetween((short)0, (short)13).WithMessage("Invalid allergen value.")
            .When(x => x.AllergenAlerts is not null);

        RuleForEach(x => x.DietaryPreferences)
            .InclusiveBetween((short)0, (short)9).WithMessage("Invalid dietary tag value.")
            .When(x => x.DietaryPreferences is not null);
    }
}
