using FluentAssertions;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.AbTesting.Commands.RecordExposure;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.UnitTests.Common;
using SwiggyClone.UnitTests.Common.Builders;

namespace SwiggyClone.UnitTests.Features.AbTesting;

public sealed class RecordExposureCommandHandlerTests
{
    private readonly List<Experiment> _experiments;
    private readonly List<ExperimentVariant> _experimentVariants;
    private readonly List<UserVariantAssignment> _assignments;
    private readonly List<ExposureEvent> _exposureEvents = [];
    private readonly IAppDbContext _db;
    private readonly RecordExposureCommandHandler _handler;

    private static readonly Guid VariantId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid AssignmentId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    public RecordExposureCommandHandlerTests()
    {
        var experiment = new ExperimentBuilder()
            .WithStatus(ExperimentStatus.Active)
            .WithKey("checkout_test")
            .Build();

        var variant = new ExperimentVariant
        {
            Id = VariantId,
            ExperimentId = TestConstants.ExperimentId,
            Key = "control",
            Name = "Control",
            AllocationPercent = 50,
            IsControl = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        var assignment = new UserVariantAssignment
        {
            Id = AssignmentId,
            UserId = TestConstants.UserId,
            ExperimentId = TestConstants.ExperimentId,
            VariantId = VariantId,
            AssignedAt = DateTimeOffset.UtcNow,
            Experiment = experiment,
            Variant = variant,
        };

        _experiments = [experiment];
        _experimentVariants = [variant];
        _assignments = [assignment];

        _db = MockDbContextFactory.Create(
            experiments: _experiments,
            experimentVariants: _experimentVariants,
            userVariantAssignments: _assignments,
            exposureEvents: _exposureEvents);

        _handler = new RecordExposureCommandHandler(_db);
    }

    [Fact]
    public async Task Handle_ValidExposure_RecordsEvent()
    {
        var command = new RecordExposureCommand(
            UserId: TestConstants.UserId,
            ExperimentKey: "checkout_test",
            Context: "checkout_page");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _exposureEvents.Should().HaveCount(1);
        _exposureEvents[0].AssignmentId.Should().Be(AssignmentId);
        _exposureEvents[0].Context.Should().Be("checkout_page");
    }

    [Fact]
    public async Task Handle_NoAssignment_ReturnsSuccessNoOp()
    {
        var command = new RecordExposureCommand(
            UserId: Guid.NewGuid(),
            ExperimentKey: "checkout_test",
            Context: null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _exposureEvents.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_UnknownExperimentKey_ReturnsSuccessNoOp()
    {
        var command = new RecordExposureCommand(
            UserId: TestConstants.UserId,
            ExperimentKey: "nonexistent_key",
            Context: null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _exposureEvents.Should().BeEmpty();
    }
}
