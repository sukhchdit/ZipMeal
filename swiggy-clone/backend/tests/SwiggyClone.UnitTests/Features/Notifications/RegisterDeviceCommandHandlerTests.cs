using FluentAssertions;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.Notifications.Commands;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.UnitTests.Common;

namespace SwiggyClone.UnitTests.Features.Notifications;

public sealed class RegisterDeviceCommandHandlerTests
{
    private readonly IAppDbContext _db;
    private readonly RegisterDeviceCommandHandler _handler;
    private readonly List<UserDevice> _userDevices = [];

    public RegisterDeviceCommandHandlerTests()
    {
        _db = MockDbContextFactory.Create(userDevices: _userDevices);
        _handler = new RegisterDeviceCommandHandler(_db);
    }

    [Fact]
    public async Task Handle_NewDevice_CreatesDeviceRecord()
    {
        // Arrange
        var command = new RegisterDeviceCommand(
            TestConstants.UserId,
            "fcm-token-abc123",
            (int)DevicePlatform.Android);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _userDevices.Should().HaveCount(1);
        _userDevices[0].UserId.Should().Be(TestConstants.UserId);
        _userDevices[0].DeviceToken.Should().Be("fcm-token-abc123");
        _userDevices[0].Platform.Should().Be(DevicePlatform.Android);
        _userDevices[0].IsActive.Should().BeTrue();
        await _db.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ExistingDevice_UpdatesExistingRecord()
    {
        // Arrange: device already registered
        var existingDevice = new UserDevice
        {
            Id = Guid.NewGuid(),
            UserId = TestConstants.UserId,
            DeviceToken = "fcm-token-existing",
            Platform = DevicePlatform.Android,
            IsActive = false,
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-30),
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-30),
        };
        _userDevices.Add(existingDevice);

        var command = new RegisterDeviceCommand(
            TestConstants.UserId,
            "fcm-token-existing",
            (int)DevicePlatform.Ios); // Platform updated

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _userDevices.Should().HaveCount(1); // No duplicate created
        _userDevices[0].IsActive.Should().BeTrue();
        _userDevices[0].Platform.Should().Be(DevicePlatform.Ios);
        await _db.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DifferentTokenSameUser_CreatesSecondDevice()
    {
        // Arrange: user has one device, registers another
        _userDevices.Add(new UserDevice
        {
            Id = Guid.NewGuid(),
            UserId = TestConstants.UserId,
            DeviceToken = "fcm-token-device1",
            Platform = DevicePlatform.Android,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        });

        var command = new RegisterDeviceCommand(
            TestConstants.UserId,
            "fcm-token-device2",
            (int)DevicePlatform.Ios);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _userDevices.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ReactivateInactiveDevice_SetsActiveTrue()
    {
        // Arrange: device was previously deactivated
        var inactiveDevice = new UserDevice
        {
            Id = Guid.NewGuid(),
            UserId = TestConstants.UserId,
            DeviceToken = "fcm-token-reactivate",
            Platform = DevicePlatform.Android,
            IsActive = false,
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-10),
            UpdatedAt = DateTimeOffset.UtcNow.AddDays(-5),
        };
        _userDevices.Add(inactiveDevice);

        var command = new RegisterDeviceCommand(
            TestConstants.UserId,
            "fcm-token-reactivate",
            (int)DevicePlatform.Android);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        inactiveDevice.IsActive.Should().BeTrue();
        inactiveDevice.UpdatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Handle_AlwaysReturnsSuccess()
    {
        // Arrange: both new and existing paths return success
        var command = new RegisterDeviceCommand(
            TestConstants.UserId,
            "any-token",
            (int)DevicePlatform.Web);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
