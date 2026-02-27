using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Addresses.DTOs;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Addresses.Commands;

internal sealed class CreateAddressCommandHandler(IAppDbContext db)
    : IRequestHandler<CreateAddressCommand, Result<AddressDto>>
{
    public async Task<Result<AddressDto>> Handle(
        CreateAddressCommand request, CancellationToken ct)
    {
        var existingCount = await db.UserAddresses
            .CountAsync(a => a.UserId == request.UserId, ct);

        var isDefault = request.IsDefault || existingCount == 0;

        // If setting as default, unset other defaults
        if (isDefault && existingCount > 0)
        {
            await db.UserAddresses
                .Where(a => a.UserId == request.UserId && a.IsDefault)
                .ExecuteUpdateAsync(s => s.SetProperty(a => a.IsDefault, false), ct);
        }

        var now = DateTimeOffset.UtcNow;
        var address = new UserAddress
        {
            Id = Guid.CreateVersion7(),
            UserId = request.UserId,
            Label = request.Label,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            State = request.State,
            PostalCode = request.PostalCode,
            Country = request.Country ?? "India",
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            IsDefault = isDefault,
            CreatedAt = now,
            UpdatedAt = now,
        };

        db.UserAddresses.Add(address);
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
