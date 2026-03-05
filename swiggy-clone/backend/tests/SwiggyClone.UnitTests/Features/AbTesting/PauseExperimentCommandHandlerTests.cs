using FluentAssertions;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.AbTesting.Commands.PauseExperiment;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.UnitTests.Common;
using SwiggyClone.UnitTests.Common.Builders;

namespace SwiggyClone.UnitTests.Features.AbTesting;

public sealed class PauseExperimentCommandHandlerTests
{
    private readonly List<Experiment> _experiments;
    private readonly IAppDbContext _db;
    private readonly PauseExperimentCommandHandler _handler;

    public PauseExperimentCommandHandlerTests()
    {
        _experiments = [new ExperimentBuilder().WithStatus(ExperimentStatus.Active).Build()];

        _db = MockDbContextFactory.Create(experiments: _experiments);
        _handler = new PauseExperimentCommandHandler(_db);
    }

    [Fact]
    public async Task Handle_ActiveExperiment_PausesSuccessfully()
    {
        var command = new PauseExperimentCommand(TestConstants.ExperimentId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _experiments[0].Status.Should().Be(ExperimentStatus.Paused);
    }

    [Fact]
    public async Task Handle_ExperimentNotFound_ReturnsFailure()
    {
        var command = new PauseExperimentCommand(Guid.NewGuid());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("EXPERIMENT_NOT_FOUND");
    }

    [Fact]
    public async Task Handle_DraftExperiment_ReturnsFailure()
    {
        _experiments.Clear();
        _experiments.Add(new ExperimentBuilder().WithStatus(ExperimentStatus.Draft).Build());

        var command = new PauseExperimentCommand(TestConstants.ExperimentId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("EXPERIMENT_INVALID_STATUS_TRANSITION");
    }

    [Fact]
    public async Task Handle_CompletedExperiment_ReturnsFailure()
    {
        _experiments.Clear();
        _experiments.Add(new ExperimentBuilder().WithStatus(ExperimentStatus.Completed).Build());

        var command = new PauseExperimentCommand(TestConstants.ExperimentId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("EXPERIMENT_INVALID_STATUS_TRANSITION");
    }
}
