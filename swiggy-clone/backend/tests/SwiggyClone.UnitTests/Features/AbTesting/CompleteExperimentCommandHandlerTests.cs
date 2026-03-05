using FluentAssertions;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.AbTesting.Commands.CompleteExperiment;
using SwiggyClone.Domain.Entities;
using SwiggyClone.Domain.Enums;
using SwiggyClone.UnitTests.Common;
using SwiggyClone.UnitTests.Common.Builders;

namespace SwiggyClone.UnitTests.Features.AbTesting;

public sealed class CompleteExperimentCommandHandlerTests
{
    private readonly List<Experiment> _experiments;
    private readonly IAppDbContext _db;
    private readonly CompleteExperimentCommandHandler _handler;

    public CompleteExperimentCommandHandlerTests()
    {
        _experiments = [new ExperimentBuilder().WithStatus(ExperimentStatus.Active).Build()];

        _db = MockDbContextFactory.Create(experiments: _experiments);
        _handler = new CompleteExperimentCommandHandler(_db);
    }

    [Fact]
    public async Task Handle_ActiveExperiment_CompletesSuccessfully()
    {
        var command = new CompleteExperimentCommand(TestConstants.ExperimentId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _experiments[0].Status.Should().Be(ExperimentStatus.Completed);
        _experiments[0].EndDate.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_DraftExperiment_CompletesSuccessfully()
    {
        _experiments.Clear();
        _experiments.Add(new ExperimentBuilder().WithStatus(ExperimentStatus.Draft).Build());

        var command = new CompleteExperimentCommand(TestConstants.ExperimentId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _experiments[0].Status.Should().Be(ExperimentStatus.Completed);
    }

    [Fact]
    public async Task Handle_PausedExperiment_CompletesSuccessfully()
    {
        _experiments.Clear();
        _experiments.Add(new ExperimentBuilder().WithStatus(ExperimentStatus.Paused).Build());

        var command = new CompleteExperimentCommand(TestConstants.ExperimentId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _experiments[0].Status.Should().Be(ExperimentStatus.Completed);
    }

    [Fact]
    public async Task Handle_ArchivedExperiment_ReturnsFailure()
    {
        _experiments.Clear();
        _experiments.Add(new ExperimentBuilder().WithStatus(ExperimentStatus.Archived).Build());

        var command = new CompleteExperimentCommand(TestConstants.ExperimentId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("EXPERIMENT_INVALID_STATUS_TRANSITION");
    }

    [Fact]
    public async Task Handle_ExperimentNotFound_ReturnsFailure()
    {
        var command = new CompleteExperimentCommand(Guid.NewGuid());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.ErrorCode.Should().Be("EXPERIMENT_NOT_FOUND");
    }
}
