using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.UnitTests.Common.Builders;

public sealed class UserBuilder
{
    private Guid _id = TestConstants.UserId;
    private string _phoneNumber = TestConstants.ValidPhone;
    private string? _email = TestConstants.ValidEmail;
    private string _fullName = TestConstants.ValidFullName;
    private string? _passwordHash = "hashed_password";
    private UserRole _role = UserRole.Customer;
    private bool _isActive = true;
    private bool _isVerified;
    private DateTimeOffset? _lastLoginAt;

    public UserBuilder WithId(Guid id) { _id = id; return this; }
    public UserBuilder WithEmail(string? email) { _email = email; return this; }
    public UserBuilder WithPhone(string phone) { _phoneNumber = phone; return this; }
    public UserBuilder WithFullName(string name) { _fullName = name; return this; }
    public UserBuilder WithPasswordHash(string? hash) { _passwordHash = hash; return this; }
    public UserBuilder WithRole(UserRole role) { _role = role; return this; }
    public UserBuilder WithIsActive(bool active) { _isActive = active; return this; }
    public UserBuilder WithIsVerified(bool verified) { _isVerified = verified; return this; }
    public UserBuilder WithLastLoginAt(DateTimeOffset? at) { _lastLoginAt = at; return this; }

    public User Build() => new()
    {
        Id = _id,
        PhoneNumber = _phoneNumber,
        Email = _email,
        FullName = _fullName,
        PasswordHash = _passwordHash,
        Role = _role,
        IsActive = _isActive,
        IsVerified = _isVerified,
        LastLoginAt = _lastLoginAt,
        CreatedAt = DateTimeOffset.UtcNow,
        UpdatedAt = DateTimeOffset.UtcNow,
    };
}
