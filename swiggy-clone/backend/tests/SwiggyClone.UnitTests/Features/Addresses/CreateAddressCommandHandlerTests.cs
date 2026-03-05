using FluentAssertions;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Addresses.Commands;
using SwiggyClone.Domain.Entities;
using SwiggyClone.UnitTests.Common;

namespace SwiggyClone.UnitTests.Features.Addresses;

public sealed class CreateAddressCommandHandlerTests
{
    private readonly IAppDbContext _db;
    private readonly CreateAddressCommandHandler _handler;
    private readonly List<UserAddress> _userAddresses = [];

    public CreateAddressCommandHandlerTests()
    {
        _db = MockDbContextFactory.Create(userAddresses: _userAddresses);
        _handler = new CreateAddressCommandHandler(_db);
    }

    [Fact]
    public async Task Handle_ValidAddress_ReturnsSuccess()
    {
        // Arrange
        var command = new CreateAddressCommand(
            TestConstants.UserId,
            "Home",
            "123 Test St",
            "Near Park",
            "Mumbai",
            "Maharashtra",
            "400001",
            "India",
            19.076,
            72.877,
            false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Label.Should().Be("Home");
        result.Value.AddressLine1.Should().Be("123 Test St");
        result.Value.City.Should().Be("Mumbai");
        result.Value.Latitude.Should().Be(19.076);
        result.Value.Longitude.Should().Be(72.877);
        _userAddresses.Should().HaveCount(1);
        await _db.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_FirstAddress_BecomesDefault()
    {
        // Arrange: no existing addresses
        var command = new CreateAddressCommand(
            TestConstants.UserId,
            "Office",
            "456 Work Ave",
            null,
            "Delhi",
            "Delhi",
            "110001",
            "India",
            28.613,
            77.209,
            false); // IsDefault = false, but should become true since it's first

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.IsDefault.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_SecondAddressNotDefault_RemainsNonDefault()
    {
        // Arrange: one existing address
        _userAddresses.Add(new UserAddress
        {
            Id = Guid.NewGuid(),
            UserId = TestConstants.UserId,
            Label = "Home",
            AddressLine1 = "123 Test St",
            Latitude = 19.0,
            Longitude = 72.0,
            IsDefault = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        });

        var command = new CreateAddressCommand(
            TestConstants.UserId,
            "Office",
            "456 Work Ave",
            null,
            "Delhi",
            "Delhi",
            "110001",
            null,
            28.613,
            77.209,
            false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.IsDefault.Should().BeFalse();
        _userAddresses.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_DefaultCountryFallback_UsesIndia()
    {
        // Arrange: country is null, should default to "India"
        var command = new CreateAddressCommand(
            TestConstants.UserId,
            "Home",
            "789 Lane",
            null,
            "Bangalore",
            "Karnataka",
            "560001",
            null, // Country is null
            12.971,
            77.594,
            false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Country.Should().Be("India");
    }

    [Fact]
    public async Task Handle_NewDefaultAddress_IsMarkedAsDefault()
    {
        // Arrange: no existing addresses, so first address auto-defaults
        var command = new CreateAddressCommand(
            TestConstants.UserId,
            "Office",
            "456 Work Ave",
            null,
            "Delhi",
            "Delhi",
            "110001",
            "India",
            28.613,
            77.209,
            true); // IsDefault = true

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.IsDefault.Should().BeTrue();
        result.Value.Label.Should().Be("Office");
    }
}
