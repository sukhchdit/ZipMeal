using FluentValidation.TestHelper;
using SwiggyClone.Application.Features.Reviews.Commands.ResolveReviewReport;
using SwiggyClone.Domain.Enums;

namespace SwiggyClone.UnitTests.Validators.Reviews;

public sealed class ResolveReviewReportCommandValidatorTests
{
    private readonly ResolveReviewReportCommandValidator _validator = new();

    private static ResolveReviewReportCommand ValidCommand() => new(
        ReportId: Guid.NewGuid(),
        AdminId: Guid.NewGuid(),
        Status: ReviewReportStatus.Dismissed,
        AdminNotes: "Not a violation");

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        var result = _validator.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyReportId_HasError()
    {
        var cmd = ValidCommand() with { ReportId = Guid.Empty };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.ReportId);
    }

    [Fact]
    public void Validate_EmptyAdminId_HasError()
    {
        var cmd = ValidCommand() with { AdminId = Guid.Empty };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.AdminId);
    }

    [Fact]
    public void Validate_StatusDismissed_HasNoError()
    {
        var cmd = ValidCommand() with { Status = ReviewReportStatus.Dismissed };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Validate_StatusActionTaken_HasNoError()
    {
        var cmd = ValidCommand() with { Status = ReviewReportStatus.ActionTaken };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Validate_StatusPending_HasError()
    {
        var cmd = ValidCommand() with { Status = ReviewReportStatus.Pending };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Validate_StatusReviewed_HasError()
    {
        var cmd = ValidCommand() with { Status = ReviewReportStatus.Reviewed };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Validate_NullAdminNotes_HasNoError()
    {
        var cmd = ValidCommand() with { AdminNotes = null };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
