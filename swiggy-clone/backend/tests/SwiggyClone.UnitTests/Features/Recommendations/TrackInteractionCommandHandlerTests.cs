using FluentAssertions;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Recommendations.Commands.TrackInteraction;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.UnitTests.Common;

namespace SwiggyClone.UnitTests.Features.Recommendations;

public sealed class TrackInteractionCommandHandlerTests
{
    private readonly List<UserInteraction> _interactions = [];
    private readonly IAppDbContext _db;
    private readonly TrackInteractionCommandHandler _handler;

    public TrackInteractionCommandHandlerTests()
    {
        _db = MockDbContextFactory.Create(userInteractions: _interactions);
        _handler = new TrackInteractionCommandHandler(_db);
    }

    [Fact]
    public async Task Handle_ValidInteraction_RecordsInteraction()
    {
        var command = new TrackInteractionCommand(
            UserId: TestConstants.UserId,
            EntityType: InteractionEntityType.Restaurant,
            EntityId: TestConstants.RestaurantId,
            InteractionType: InteractionType.View);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _interactions.Should().HaveCount(1);
        _interactions[0].UserId.Should().Be(TestConstants.UserId);
        _interactions[0].EntityType.Should().Be(InteractionEntityType.Restaurant);
        _interactions[0].EntityId.Should().Be(TestConstants.RestaurantId);
        _interactions[0].InteractionType.Should().Be(InteractionType.View);
        _interactions[0].Id.Should().NotBeEmpty();
        _interactions[0].CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Handle_MenuItemClick_RecordsInteraction()
    {
        var command = new TrackInteractionCommand(
            UserId: TestConstants.UserId,
            EntityType: InteractionEntityType.MenuItem,
            EntityId: TestConstants.MenuItemId,
            InteractionType: InteractionType.Click);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _interactions.Should().HaveCount(1);
        _interactions[0].EntityType.Should().Be(InteractionEntityType.MenuItem);
        _interactions[0].InteractionType.Should().Be(InteractionType.Click);
    }

    [Fact]
    public async Task Handle_OrderInteraction_RecordsInteraction()
    {
        var command = new TrackInteractionCommand(
            UserId: TestConstants.UserId,
            EntityType: InteractionEntityType.Restaurant,
            EntityId: TestConstants.RestaurantId,
            InteractionType: InteractionType.Order);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _interactions.Should().HaveCount(1);
        _interactions[0].InteractionType.Should().Be(InteractionType.Order);
    }

    [Fact]
    public async Task Handle_MultipleInteractions_RecordsAll()
    {
        var command1 = new TrackInteractionCommand(
            TestConstants.UserId, InteractionEntityType.Restaurant,
            TestConstants.RestaurantId, InteractionType.View);
        var command2 = new TrackInteractionCommand(
            TestConstants.UserId, InteractionEntityType.MenuItem,
            TestConstants.MenuItemId, InteractionType.Click);

        await _handler.Handle(command1, CancellationToken.None);
        var result = await _handler.Handle(command2, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _interactions.Should().HaveCount(2);
    }
}
