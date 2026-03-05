using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwiggyClone.Api.Contracts.AbTesting;
using SwiggyClone.Application.Common.Interfaces;
using SwiggyClone.Application.Features.AbTesting.Commands.ActivateExperiment;
using SwiggyClone.Application.Features.AbTesting.Commands.CompleteExperiment;
using SwiggyClone.Application.Features.AbTesting.Commands.CreateExperiment;
using SwiggyClone.Application.Features.AbTesting.Commands.PauseExperiment;
using SwiggyClone.Application.Features.AbTesting.Commands.RecordConversion;
using SwiggyClone.Application.Features.AbTesting.Commands.RecordExposure;
using SwiggyClone.Application.Features.AbTesting.Commands.UpdateExperiment;
using SwiggyClone.Application.Features.AbTesting.Queries.GetExperimentById;
using SwiggyClone.Application.Features.AbTesting.Queries.GetExperimentResults;
using SwiggyClone.Application.Features.AbTesting.Queries.GetExperiments;
using SwiggyClone.Application.Features.AbTesting.Queries.GetUserAssignments;

namespace SwiggyClone.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
public sealed class AbTestingController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public AbTestingController(ISender sender, ICurrentUserService currentUser)
    {
        _sender = sender;
        _currentUser = currentUser;
    }

    // ─────────────────── Admin Endpoints ──────────────────────────

    /// <summary>Create a new experiment with variants.</summary>
    [HttpPost("api/v{version:apiVersion}/ab-testing/experiments")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateExperiment(
        [FromBody] CreateExperimentRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var variantInputs = request.Variants.Select(v =>
            new CreateExperimentVariantInput(v.Key, v.Name, v.AllocationPercent, v.ConfigJson, v.IsControl)).ToList();

        var result = await _sender.Send(
            new CreateExperimentCommand(userId, request.Key, request.Name, request.Description,
                request.TargetAudience, request.StartDate, request.EndDate,
                request.GoalDescription, variantInputs), ct);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetExperimentById), new { id = result.Value.Id }, result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>List all experiments (paginated, optional status filter).</summary>
    [HttpGet("api/v{version:apiVersion}/ab-testing/experiments")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetExperiments(
        [FromQuery] int? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(new GetExperimentsQuery(status, page, pageSize), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get a single experiment with variants.</summary>
    [HttpGet("api/v{version:apiVersion}/ab-testing/experiments/{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetExperimentById(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new GetExperimentByIdQuery(id), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Update a draft experiment.</summary>
    [HttpPut("api/v{version:apiVersion}/ab-testing/experiments/{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateExperiment(
        Guid id,
        [FromBody] UpdateExperimentRequest request,
        CancellationToken ct)
    {
        var result = await _sender.Send(
            new UpdateExperimentCommand(id, request.Name, request.Description,
                request.TargetAudience, request.StartDate, request.EndDate,
                request.GoalDescription), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Activate a draft or paused experiment.</summary>
    [HttpPost("api/v{version:apiVersion}/ab-testing/experiments/{id:guid}/activate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ActivateExperiment(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new ActivateExperimentCommand(id), ct);

        return result.IsSuccess
            ? Ok()
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Pause an active experiment.</summary>
    [HttpPost("api/v{version:apiVersion}/ab-testing/experiments/{id:guid}/pause")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PauseExperiment(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new PauseExperimentCommand(id), ct);

        return result.IsSuccess
            ? Ok()
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Complete an experiment.</summary>
    [HttpPost("api/v{version:apiVersion}/ab-testing/experiments/{id:guid}/complete")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CompleteExperiment(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new CompleteExperimentCommand(id), ct);

        return result.IsSuccess
            ? Ok()
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Get statistical results for an experiment.</summary>
    [HttpGet("api/v{version:apiVersion}/ab-testing/experiments/{id:guid}/results")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetExperimentResults(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new GetExperimentResultsQuery(id), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : NotFound(new { result.ErrorCode, result.ErrorMessage });
    }

    // ─────────────────── User Endpoints ───────────────────────────

    /// <summary>Get all active experiment assignments for the current user.</summary>
    [HttpGet("api/v{version:apiVersion}/ab-testing/assignments")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAssignments(CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(new GetUserAssignmentsQuery(userId), ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Record an exposure event.</summary>
    [HttpPost("api/v{version:apiVersion}/ab-testing/exposure")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RecordExposure(
        [FromBody] RecordExposureRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new RecordExposureCommand(userId, request.ExperimentKey, request.Context), ct);

        return result.IsSuccess
            ? Ok()
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }

    /// <summary>Record a conversion event.</summary>
    [HttpPost("api/v{version:apiVersion}/ab-testing/conversion")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RecordConversion(
        [FromBody] RecordConversionRequest request,
        CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var result = await _sender.Send(
            new RecordConversionCommand(userId, request.ExperimentKey, request.GoalKey, request.Value), ct);

        return result.IsSuccess
            ? Ok()
            : BadRequest(new { result.ErrorCode, result.ErrorMessage });
    }
}
