using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Addresses.DTOs;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Addresses.Queries;

internal sealed class GetAddressesQueryHandler(IAppDbContext db)
    : IRequestHandler<GetAddressesQuery, Result<List<AddressDto>>>
{
    public async Task<Result<List<AddressDto>>> Handle(
        GetAddressesQuery request, CancellationToken ct)
    {
        var addresses = await db.UserAddresses
            .AsNoTracking()
            .Where(a => a.UserId == request.UserId)
            .OrderByDescending(a => a.IsDefault)
            .ThenByDescending(a => a.CreatedAt)
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
            .ToListAsync(ct);

        return Result<List<AddressDto>>.Success(addresses);
    }
}
