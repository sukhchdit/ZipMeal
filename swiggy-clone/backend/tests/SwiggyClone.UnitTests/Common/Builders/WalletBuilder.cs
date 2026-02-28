using SwiggyClone.Domain.Entities;

namespace SwiggyClone.UnitTests.Common.Builders;

public sealed class WalletBuilder
{
    private Guid _id = TestConstants.WalletId;
    private Guid _userId = TestConstants.UserId;
    private int _balancePaise;

    public WalletBuilder WithId(Guid id) { _id = id; return this; }
    public WalletBuilder WithUserId(Guid userId) { _userId = userId; return this; }
    public WalletBuilder WithBalance(int balancePaise) { _balancePaise = balancePaise; return this; }

    public Wallet Build() => new()
    {
        Id = _id,
        UserId = _userId,
        BalancePaise = _balancePaise,
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow,
    };
}
