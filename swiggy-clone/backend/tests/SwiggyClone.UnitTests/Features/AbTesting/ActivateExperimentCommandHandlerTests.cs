using FluentAssertions;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.AbTesting.Commands.ActivateExperiment;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.UnitTests.Common;
using SwiggyClone.UnitTests.Common.Builders;

namespace SwiggyClone.UnitTests.Features.AbTesting;

public sealed class ActivateExperimentCommandHandlerTests
{
    private readonly List<Experiment> _experiments;
    private readonly IAppDbContext _db;
    private readonly ActivateExperimentCommandHandler _handler;

    public ActivateExperimentCommandHandlerTests()
    {
        _experiments = [new ExperimentBuilder().WithStatus(ExperimentStatus.Draft).Build()];

        _db = MockDbContextFactory.Create(experiments: _experiments);
        _handler = new ActivateExperimentCommandHandler(_db);
    }

    [Fact]
    public async Task Handle_DraftExperiment_ActivatesSuccessfully()
    {
        var command = new ActivateExperimentCommand(TestConstants.ExperimentId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _experiments[0].Status.Should().Be(ExperimentStatus.Active);
        _experiments[0].StartDate.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_PausedExperiment_ActivatesSuccessfully()
    {
        _experiments.Clear();
        _experiments.Add(new ExperimentBuilder().WithStatus(ExperimentStatus.Paused).Build());

        var command = new ActivateExperimentCommand(TestConstants.ExperimentId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _experiments[0].Status.Should().Be(ExperimentStatus.Active);
    }

    [Fact]
    public async Task Handle_ExperimentNotFound_ReturnsFailure()
    {
        var command = new ActivateExperimentCommand(Guid.NewGuid());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("EXPERIMENT_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_CompletedExperiment_ReturnsFailure()
    {
        _experiments.Clear();
        _experiments.Add(new ExperimentBuilder().WithStatus(ExperimentStatus.Completed).Build());

        var command = new ActivateExperimentCommand(TestConstants.ExperimentId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("EXPERIMENT_INVALID_STATUS_TRANSITION");
    }

    [Fact]
    public async Task Handle_ArchivedExperiment_ReturnsFailure()
    {
        _experiments.Clear();
        _experiments.Add(new ExperimentBuilder().WithStatus(ExperimentStatus.Archived).Build());

        var command = new ActivateExperimentCommand(TestConstants.ExperimentId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("EXPERIMENT_INVALID_STATUS_TRANSITION");
    }
}
