using FluentAssertions;
using MediatR;
using NSubstitute;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Social.Commands;
using SwiggyClone.Application.Features.Social.Notifications;
using SwiggyClone.Domain.Entities;
using SwiggyClone.UnitTests.Common;
using SwiggyClone.UnitTests.Common.Builders;

namespace SwiggyClone.UnitTests.Features.Social;

public sealed class FollowUserCommandHandlerTests
{
    private readonly IAppDbContext _db;
    private readonly IPublisher _publisher;
    private readonly FollowUserCommandHandler _handler;

    private readonly List<User> _users = [];
    private readonly List<UserFollow> _userFollows = [];

    public FollowUserCommandHandlerTests()
    {
        _db = MockDbContextFactory.Create(
            users: _users,
            userFollows: _userFollows);

        _publisher = Substitute.For<IPublisher>();
        _handler = new FollowUserCommandHandler(_db, _publisher);
    }

    [Fact]
    public async Task Handle_SelfFollow_ReturnsFailure()
    {
        // Arrange: user tries to follow themselves
        var command = new FollowUserCommand(TestConstants.UserId, TestConstants.UserId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("CANNOT_FOLLOW_SELF");
    }

    [Fact]
    public async Task Handle_TargetUserNotFound_ReturnsFailure()
    {
        // Arrange: target user does not exist in DB
        var command = new FollowUserCommand(TestConstants.UserId, Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("USER_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_AlreadyFollowing_ReturnsFailure()
    {
        // Arrange
        var targetUserId = TestConstants.ParticipantUserId;
        var targetUser = new UserBuilder().WithId(targetUserId).WithEmail("target@test.com").Build();
        _users.Add(targetUser);

        _userFollows.Add(new UserFollow
        {
            FollowerId = TestConstants.UserId,
            FollowingId = targetUserId,
            CreatedAt = DateTimeOffset.UtcNow,
        });

        var command = new FollowUserCommand(TestConstants.UserId, targetUserId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("ALREADY_FOLLOWING");
    }

    [Fact]
    public async Task Handle_ValidFollow_ReturnsSuccess()
    {
        // Arrange
        var targetUserId = TestConstants.ParticipantUserId;
        var targetUser = new UserBuilder().WithId(targetUserId).WithEmail("target@test.com").Build();
        _users.Add(targetUser);

        var command = new FollowUserCommand(TestConstants.UserId, targetUserId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _userFollows.Should().HaveCount(1);
        _userFollows[0].FollowerId.Should().Be(TestConstants.UserId);
        _userFollows[0].FollowingId.Should().Be(targetUserId);
        await _db.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidFollow_PublishesUserFollowedNotification()
    {
        // Arrange
        var targetUserId = TestConstants.ParticipantUserId;
        var targetUser = new UserBuilder().WithId(targetUserId).WithEmail("target@test.com").Build();
        _users.Add(targetUser);

        var command = new FollowUserCommand(TestConstants.UserId, targetUserId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _publisher.Received(1).Publish(
            Arg.Is<UserFollowedNotification>(n =>
                n.FollowerId == TestConstants.UserId &&
                n.FollowingId == targetUserId),
            Arg.Any<CancellationToken>());
    }
}
