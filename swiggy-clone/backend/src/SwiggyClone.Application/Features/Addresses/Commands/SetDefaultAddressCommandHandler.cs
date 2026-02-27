using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Addresses.Commands;

internal sealed class SetDefaultAddressCommandHandler(IAppDbContext db)
    : IRequestHandler<SetDefaultAddressCommand, Result>
{
    public async Task<Result> Handle(SetDefaultAddressCommand request, CancellationToken ct)
    {
        var address = await db.UserAddresses
            .FirstOrDefaultAsync(a => a.Id == request.AddressId && a.UserId == request.UserId, ct);

        if (address is null)
            return Result.Failure("ADDRESS_NOT_FOUND", "Address not found.");

        // Unset all defaults for this user
        await db.UserAddresses
            .Where(a => a.UserId == request.UserId && a.IsDefault)
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.IsDefault, false), ct);

        address.IsDefault = true;
        await db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
