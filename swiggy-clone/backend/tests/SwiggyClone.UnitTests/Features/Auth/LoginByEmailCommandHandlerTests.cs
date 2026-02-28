using FluentAssertions;
using NSubstitute;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Auth.Commands;
using SwiggyClone.Domain.Entities;
using SwiggyClone.UnitTests.Common;
using SwiggyClone.UnitTests.Common.Builders;

namespace SwiggyClone.UnitTests.Features.Auth;

public sealed class LoginByEmailCommandHandlerTests
{
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();

    public LoginByEmailCommandHandlerTests()
    {
        _passwordHasher.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
        _tokenService.GenerateAccessToken(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<string>())
            .Returns("access_token");
        _tokenService.GenerateRefreshToken().Returns("refresh_token");
        _tokenService.HashToken(Arg.Any<string>()).Returns("hashed_refresh");
    }

    private LoginByEmailCommandHandler CreateHandler(List<User>? users = null, List<RefreshToken>? refreshTokens = null)
    {
        var db = MockDbContextFactory.Create(
            users: users ?? [],
            refreshTokens: refreshTokens ?? []);
        return new LoginByEmailCommandHandler(db, _passwordHasher, _tokenService);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsFailure()
    {
        var handler = CreateHandler();
        var command = new LoginByEmailCommand("unknown@example.com", "password");

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("INVALID_CREDENTIALS");
    }

    [Fact]
    public async Task Handle_AccountDisabled_ReturnsFailure()
    {
        var user = new UserBuilder().WithEmail("disabled@example.com").WithIsActive(false).Build();
        var handler = CreateHandler(users: [user]);
        var command = new LoginByEmailCommand("disabled@example.com", "password");

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("ACCOUNT_DISABLED");
    }

    [Fact]
    public async Task Handle_NoPassword_ReturnsFailure()
    {
        var user = new UserBuilder().WithEmail("otp@example.com").WithPasswordHash(null).Build();
        var handler = CreateHandler(users: [user]);
        var command = new LoginByEmailCommand("otp@example.com", "password");

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("NO_PASSWORD");
    }

    [Fact]
    public async Task Handle_WrongPassword_ReturnsFailure()
    {
        var user = new UserBuilder().WithEmail("user@example.com").Build();
        _passwordHasher.Verify("wrong", user.PasswordHash!).Returns(false);
        var handler = CreateHandler(users: [user]);
        var command = new LoginByEmailCommand("user@example.com", "wrong");

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("INVALID_CREDENTIALS");
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsSuccess()
    {
        var user = new UserBuilder().WithEmail("user@example.com").Build();
        var handler = CreateHandler(users: [user]);
        var command = new LoginByEmailCommand("user@example.com", "correct");

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be("access_token");
    }

    [Fact]
    public async Task Handle_ValidCredentials_UpdatesLastLogin()
    {
        var user = new UserBuilder().WithEmail("user@example.com").WithLastLoginAt(null).Build();
        var handler = CreateHandler(users: [user]);
        var command = new LoginByEmailCommand("user@example.com", "correct");

        await handler.Handle(command, CancellationToken.None);

        user.LastLoginAt.Should().NotBeNull();
        user.LastLoginAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }
}
