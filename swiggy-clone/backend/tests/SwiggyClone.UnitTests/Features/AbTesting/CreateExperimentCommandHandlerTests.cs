using FluentAssertions;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.AbTesting.Commands.CreateExperiment;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.UnitTests.Common;
using SwiggyClone.UnitTests.Common.Builders;

namespace SwiggyClone.UnitTests.Features.AbTesting;

public sealed class CreateExperimentCommandHandlerTests
{
    private readonly List<Experiment> _experiments = [];
    private readonly List<ExperimentVariant> _experimentVariants = [];
    private readonly IAppDbContext _db;
    private readonly CreateExperimentCommandHandler _handler;

    public CreateExperimentCommandHandlerTests()
    {
        _db = MockDbContextFactory.Create(
            experiments: _experiments,
            experimentVariants: _experimentVariants);

        _handler = new CreateExperimentCommandHandler(_db);
    }

    private static readonly IReadOnlyList<CreateExperimentVariantInput> ValidVariants =
    [
        new("control", "Control", 50, null, true),
        new("treatment_a", "Treatment A", 50, """{"buttonColor":"red"}""", false),
    ];

    private static CreateExperimentCommand ValidCommand() => new(
        CreatedByUserId: TestConstants.AdminId,
        Key: "checkout_flow_v2",
        Name: "Checkout Flow V2",
        Description: "Test new checkout flow",
        TargetAudience: "all_users",
        StartDate: null,
        EndDate: null,
        GoalDescription: "Increase conversion rate",
        Variants: ValidVariants);

    [Fact]
    public async Task Handle_ValidRequest_CreatesExperiment()
    {
        var command = ValidCommand();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Key.Should().Be("checkout_flow_v2");
        result.Value.Name.Should().Be("Checkout Flow V2");
        result.Value.Description.Should().Be("Test new checkout flow");
        result.Value.Status.Should().Be((int)ExperimentStatus.Draft);
        result.Value.CreatedByUserId.Should().Be(TestConstants.AdminId);
        result.Value.Variants.Should().HaveCount(2);
        result.Value.Variants[0].Key.Should().Be("control");
        result.Value.Variants[0].IsControl.Should().BeTrue();
        result.Value.Variants[1].Key.Should().Be("treatment_a");
        result.Value.Variants[1].AllocationPercent.Should().Be(50);
        _experiments.Should().HaveCount(1);
        _experimentVariants.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_DuplicateKey_ReturnsFailure()
    {
        _experiments.Add(new ExperimentBuilder()
            .WithKey("checkout_flow_v2")
            .Build());

        var command = ValidCommand();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("EXPERIMENT_KEY_DUPLICATE");
    }
}
