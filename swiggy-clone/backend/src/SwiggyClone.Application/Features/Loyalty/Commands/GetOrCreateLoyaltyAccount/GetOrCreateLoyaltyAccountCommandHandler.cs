using MediatR;
using Microsoft.EntityFrameworkCore;
using SwiggyClone.Application.Common.Interfaces;

namespace SwiggyClone.Application.Features.Loyalty.Commands.GetOrCreateLoyaltyAccount;

internal sealed class GetOrCreateLoyaltyAccountCommandHandler(IAppDbContext db)
    : IRequestHandler<GetOrCreateLoyaltyAccountCommand, Domain.Entities.LoyaltyAccount>
{
    public async Task<Domain.Entities.LoyaltyAccount> Handle(
        GetOrCreateLoyaltyAccountCommand request, CancellationToken ct)
    {
        var account = await db.LoyaltyAccounts
            .FirstOrDefaultAsync(a => a.UserId == request.UserId, ct);

        if (account is not null)
        {
            return account;
        }

        account = Domain.Entities.LoyaltyAccount.Create(request.UserId);
        db.LoyaltyAccounts.Add(account);
        await db.SaveChangesAsync(ct);

        return account;
    }
}
