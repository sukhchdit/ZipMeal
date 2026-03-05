using FluentAssertions;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.AbTesting.Commands.RecordConversion;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.UnitTests.Common;
using SwiggyClone.UnitTests.Common.Builders;

namespace SwiggyClone.UnitTests.Features.AbTesting;

public sealed class RecordConversionCommandHandlerTests
{
    private readonly List<Experiment> _experiments;
    private readonly List<ExperimentVariant> _experimentVariants;
    private readonly List<UserVariantAssignment> _assignments;
    private readonly List<ConversionEvent> _conversionEvents = [];
    private readonly IAppDbContext _db;
    private readonly RecordConversionCommandHandler _handler;

    private static readonly Guid VariantId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid AssignmentId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    public RecordConversionCommandHandlerTests()
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
            conversionEvents: _conversionEvents);

        _handler = new RecordConversionCommandHandler(_db);
    }

    [Fact]
    public async Task Handle_ValidConversion_RecordsEvent()
    {
        var command = new RecordConversionCommand(
            UserId: TestConstants.UserId,
            ExperimentKey: "checkout_test",
            GoalKey: "purchase",
            Value: 599.99m);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _conversionEvents.Should().HaveCount(1);
        _conversionEvents[0].AssignmentId.Should().Be(AssignmentId);
        _conversionEvents[0].GoalKey.Should().Be("purchase");
        _conversionEvents[0].Value.Should().Be(599.99m);
    }

    [Fact]
    public async Task Handle_NullValue_RecordsEventWithoutValue()
    {
        var command = new RecordConversionCommand(
            UserId: TestConstants.UserId,
            ExperimentKey: "checkout_test",
            GoalKey: "click_cta",
            Value: null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _conversionEvents.Should().HaveCount(1);
        _conversionEvents[0].Value.Should().BeNull();
    }

    [Fact]
    public async Task Handle_NoAssignment_ReturnsSuccessNoOp()
    {
        var command = new RecordConversionCommand(
            UserId: Guid.NewGuid(),
            ExperimentKey: "checkout_test",
            GoalKey: "purchase",
            Value: 100m);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _conversionEvents.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_UnknownExperimentKey_ReturnsSuccessNoOp()
    {
        var command = new RecordConversionCommand(
            UserId: TestConstants.UserId,
            ExperimentKey: "nonexistent_key",
            GoalKey: "purchase",
            Value: 100m);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _conversionEvents.Should().BeEmpty();
    }
}
