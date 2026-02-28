using FluentAssertions;
using SwiggyClone.Application.Common.Helpers;
using SwiggyClone.Domain.Entities;
using SwiggyClone.UnitTests.Common;
using SwiggyClone.UnitTests.Common.Builders;

namespace SwiggyClone.UnitTests.Application.Helpers;

public sealed class RestaurantOwnershipHelperTests
{
    [Fact]
    public async Task VerifyOwnership_RestaurantNotFound_ReturnsFailure()
    {
        var db = MockDbContextFactory.Create();

        var result = await RestaurantOwnershipHelper.VerifyOwnership(
            db, Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("RESTAURANT_NOT_FOUND");
    }

    [Fact]
    public async Task VerifyOwnership_WrongOwner_ReturnsFailure()
    {
        var restaurant = new RestaurantBuilder().Build();
        var db = MockDbContextFactory.Create(restaurants: new List<Restaurant> { restaurant });

        var result = await RestaurantOwnershipHelper.VerifyOwnership(
            db, restaurant.Id, Guid.NewGuid(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("FORBIDDEN");
    }

    [Fact]
    public async Task VerifyOwnership_CorrectOwner_ReturnsSuccess()
    {
        var restaurant = new RestaurantBuilder().Build();
        var db = MockDbContextFactory.Create(restaurants: new List<Restaurant> { restaurant });

        var result = await RestaurantOwnershipHelper.VerifyOwnership(
            db, restaurant.Id, restaurant.OwnerId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(restaurant);
    }
}
