using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Shared;

namespace SwiggyClone.Application.Features.Addresses.Commands;

internal sealed class DeleteAddressCommandHandler(IAppDbContext db)
    : IRequestHandler<DeleteAddressCommand, Result>
{
    public async Task<Result> Handle(DeleteAddressCommand request, CancellationToken ct)
    {
        var address = await db.UserAddresses
            .FirstOrDefaultAsync(a => a.Id == request.AddressId && a.UserId == request.UserId, ct);

        if (address is null)
            return Result.Failure("ADDRESS_NOT_FOUND", "Address not found.");

        var wasDefault = address.IsDefault;
        address.SoftDelete();

        await db.SaveChangesAsync(ct);

        // If deleted address was the default, promote the most recent remaining
        if (wasDefault)
        {
            var nextDefault = await db.UserAddresses
                .Where(a => a.UserId == request.UserId)
                .OrderByDescending(a => a.CreatedAt)
                .FirstOrDefaultAsync(ct);

            if (nextDefault is not null)
            {
                nextDefault.IsDefault = true;
                await db.SaveChangesAsync(ct);
            }
        }

        return Result.Success();
    }
}
