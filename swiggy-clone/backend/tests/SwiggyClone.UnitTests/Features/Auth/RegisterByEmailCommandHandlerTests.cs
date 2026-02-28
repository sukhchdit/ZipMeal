using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Auth.Commands;
using SwiggyClone.Domain.Entities;
using SwiggyClone.UnitTests.Common;
using SwiggyClone.UnitTests.Common.Builders;

namespace SwiggyClone.UnitTests.Features.Auth;

public sealed class RegisterByEmailCommandHandlerTests
{
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly ISender _sender = Substitute.For<ISender>();
    private readonly IEventBus _eventBus = Substitute.For<IEventBus>();
    private readonly ILogger<RegisterByEmailCommandHandler> _logger = Substitute.For<ILogger<RegisterByEmailCommandHandler>>();
    private readonly List<User> _users = [];
    private readonly List<RefreshToken> _refreshTokens = [];

    public RegisterByEmailCommandHandlerTests()
    {
        _passwordHasher.Hash(Arg.Any<string>()).Returns("hashed_pw");
        _tokenService.GenerateAccessToken(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<string>())
            .Returns("access_token");
        _tokenService.GenerateRefreshToken().Returns("refresh_token");
        _tokenService.HashToken(Arg.Any<string>()).Returns("hashed_refresh");
    }

    private RegisterByEmailCommandHandler CreateHandler()
    {
        var db = MockDbContextFactory.Create(users: _users, refreshTokens: _refreshTokens);
        return new RegisterByEmailCommandHandler(db, _passwordHasher, _tokenService, _sender, _eventBus, _logger);
    }

    [Fact]
    public async Task Handle_EmailTaken_ReturnsFailure()
    {
        _users.Add(new UserBuilder().WithEmail("taken@example.com").Build());
        var handler = CreateHandler();
        var command = new RegisterByEmailCommand("taken@example.com", "P@ssw0rd!", "Test", "+919876543210");

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("EMAIL_TAKEN");
    }

    [Fact]
    public async Task Handle_PhoneTaken_ReturnsFailure()
    {
        _users.Add(new UserBuilder().WithPhone("+919876543210").Build());
        var handler = CreateHandler();
        var command = new RegisterByEmailCommand("new@example.com", "P@ssw0rd!", "Test", "+919876543210");

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("PHONE_TAKEN");
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesUser()
    {
        var handler = CreateHandler();
        var command = new RegisterByEmailCommand("new@example.com", "P@ssw0rd!", "New User", "+911234567890");

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _users.Should().HaveCount(1);
        _users[0].Email.Should().Be("new@example.com");
        _users[0].FullName.Should().Be("New User");
    }

    [Fact]
    public async Task Handle_ValidRequest_HashesPassword()
    {
        var handler = CreateHandler();
        var command = new RegisterByEmailCommand("new@example.com", "P@ssw0rd!", "New User", "+911234567890");

        await handler.Handle(command, CancellationToken.None);

        _passwordHasher.Received(1).Hash("P@ssw0rd!");
        _users[0].PasswordHash.Should().Be("hashed_pw");
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsAuthResponse()
    {
        var handler = CreateHandler();
        var command = new RegisterByEmailCommand("new@example.com", "P@ssw0rd!", "New User", "+911234567890");

        var result = await handler.Handle(command, CancellationToken.None);

        result.Value.AccessToken.Should().Be("access_token");
        result.Value.RefreshToken.Should().Be("refresh_token");
        result.Value.User.Email.Should().Be("new@example.com");
    }

    [Fact]
    public async Task Handle_ValidRequest_CreatesRefreshToken()
    {
        var handler = CreateHandler();
        var command = new RegisterByEmailCommand("new@example.com", "P@ssw0rd!", "New User", "+911234567890");

        await handler.Handle(command, CancellationToken.None);

        _refreshTokens.Should().HaveCount(1);
        _refreshTokens[0].TokenHash.Should().Be("hashed_refresh");
    }
}
