using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Addresses.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Addresses.Commands;

internal sealed class UpdateAddressCommandHandler(IAppDbContext db)
    : IRequestHandler<UpdateAddressCommand, Result<AddressDto>>
{
    public async Task<Result<AddressDto>> Handle(
        UpdateAddressCommand request, CancellationToken ct)
    {
        var address = await db.UserAddresses
            .FirstOrDefaultAsync(a => a.Id == request.AddressId && a.UserId == request.UserId, ct);

        if (address is null)
            return Result<AddressDto>.Failure("ADDRESS_NOT_FOUND", "Address not found.");

        address.Label = request.Label;
        address.AddressLine1 = request.AddressLine1;
        address.AddressLine2 = request.AddressLine2;
        address.City = request.City;
        address.State = request.State;
        address.PostalCode = request.PostalCode;
        address.Country = request.Country ?? "India";
        address.Latitude = request.Latitude;
        address.Longitude = request.Longitude;
        address.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);

        var dto = new AddressDto(
            address.Id,
            address.Label,
            address.AddressLine1,
            address.AddressLine2,
            address.City,
            address.State,
            address.PostalCode,
            address.Country,
            address.Latitude,
            address.Longitude,
            address.IsDefault,
            address.CreatedAt);

        return Result<AddressDto>.Success(dto);
    }
}
