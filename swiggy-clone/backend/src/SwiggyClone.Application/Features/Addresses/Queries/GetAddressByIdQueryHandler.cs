using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Addresses.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Addresses.Queries;

internal sealed class GetAddressByIdQueryHandler(IAppDbContext db)
    : IRequestHandler<GetAddressByIdQuery, Result<AddressDto>>
{
    public async Task<Result<AddressDto>> Handle(
        GetAddressByIdQuery request, CancellationToken ct)
    {
        var address = await db.UserAddresses
            .AsNoTracking()
            .Where(a => a.Id == request.AddressId && a.UserId == request.UserId)
            .Select(a => new AddressDto(
                a.Id,
                a.Label,
                a.AddressLine1,
                a.AddressLine2,
                a.City,
                a.State,
                a.PostalCode,
                a.Country,
                a.Latitude,
                a.Longitude,
                a.IsDefault,
                a.CreatedAt))
            .FirstOrDefaultAsync(ct);

        return address is not null
            ? Result<AddressDto>.Success(address)
            : Result<AddressDto>.Failure("ADDRESS_NOT_FOUND", "Address not found.");
    }
}
