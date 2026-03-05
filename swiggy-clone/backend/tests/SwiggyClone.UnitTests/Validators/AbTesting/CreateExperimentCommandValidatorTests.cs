using FluentValidation.TestHelper;
using SwiggyClone.Application.Features.AbTesting.Commands.CreateExperiment;

namespace SwiggyClone.UnitTests.Validators.AbTesting;

public sealed class CreateExperimentCommandValidatorTests
{
    private readonly CreateExperimentCommandValidator _validator = new();

    private static readonly IReadOnlyList<CreateExperimentVariantInput> ValidVariants =
    [
        new("control", "Control", 50, null, true),
        new("treatment", "Treatment", 50, """{"color":"blue"}""", false),
    ];

    private static CreateExperimentCommand ValidCommand() => new(
        CreatedByUserId: Guid.NewGuid(),
        Key: "checkout_flow_v2",
        Name: "Checkout Flow V2",
        Description: "Test new checkout",
        TargetAudience: "all",
        StartDate: null,
        EndDate: null,
        GoalDescription: "Increase conversion",
        Variants: ValidVariants);

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        var result = _validator.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    // ── CreatedByUserId ──

    [Fact]
    public void Validate_EmptyCreatedByUserId_HasError()
    {
        var cmd = ValidCommand() with { CreatedByUserId = Guid.Empty };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.CreatedByUserId);
    }

    // ── Key ──

    [Fact]
    public void Validate_EmptyKey_HasError()
    {
        var cmd = ValidCommand() with { Key = "" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Key);
    }

    [Fact]
    public void Validate_KeyTooLong_HasError()
    {
        var cmd = ValidCommand() with { Key = new string('a', 101) };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Key);
    }

    [Fact]
    public void Validate_KeyWithUppercase_HasError()
    {
        var cmd = ValidCommand() with { Key = "UPPERCASE_KEY" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Key);
    }

    [Fact]
    public void Validate_KeyWithSpaces_HasError()
    {
        var cmd = ValidCommand() with { Key = "has spaces" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Key);
    }

    [Fact]
    public void Validate_KeyWithHyphens_HasError()
    {
        var cmd = ValidCommand() with { Key = "has-hyphens" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Key);
    }

    [Fact]
    public void Validate_ValidKeyWithUnderscores_HasNoError()
    {
        var cmd = ValidCommand() with { Key = "valid_key_123" };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.Key);
    }

    // ── Name ──

    [Fact]
    public void Validate_EmptyName_HasError()
    {
        var cmd = ValidCommand() with { Name = "" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_NameTooLong_HasError()
    {
        var cmd = ValidCommand() with { Name = new string('x', 201) };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    // ── Description ──

    [Fact]
    public void Validate_DescriptionTooLong_HasError()
    {
        var cmd = ValidCommand() with { Description = new string('x', 2001) };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_NullDescription_HasNoError()
    {
        var cmd = ValidCommand() with { Description = null };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    // ── TargetAudience ──

    [Fact]
    public void Validate_TargetAudienceTooLong_HasError()
    {
        var cmd = ValidCommand() with { TargetAudience = new string('x', 501) };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.TargetAudience);
    }

    // ── GoalDescription ──

    [Fact]
    public void Validate_GoalDescriptionTooLong_HasError()
    {
        var cmd = ValidCommand() with { GoalDescription = new string('x', 1001) };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.GoalDescription);
    }

    // ── Variants ──

    [Fact]
    public void Validate_EmptyVariants_HasError()
    {
        var cmd = ValidCommand() with { Variants = [] };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Variants);
    }

    [Fact]
    public void Validate_AllocationsSumNot100_HasError()
    {
        IReadOnlyList<CreateExperimentVariantInput> variants =
        [
            new("control", "Control", 40, null, true),
            new("treatment", "Treatment", 40, null, false),
        ];
        var cmd = ValidCommand() with { Variants = variants };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveAnyValidationError()
            .WithErrorMessage("Variant allocations must sum to 100.");
    }

    [Fact]
    public void Validate_NoControlVariant_HasError()
    {
        IReadOnlyList<CreateExperimentVariantInput> variants =
        [
            new("a", "A", 50, null, false),
            new("b", "B", 50, null, false),
        ];
        var cmd = ValidCommand() with { Variants = variants };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveAnyValidationError()
            .WithErrorMessage("Exactly one variant must be marked as control.");
    }

    [Fact]
    public void Validate_MultipleControlVariants_HasError()
    {
        IReadOnlyList<CreateExperimentVariantInput> variants =
        [
            new("a", "A", 50, null, true),
            new("b", "B", 50, null, true),
        ];
        var cmd = ValidCommand() with { Variants = variants };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveAnyValidationError()
            .WithErrorMessage("Exactly one variant must be marked as control.");
    }

    // ── Variant child rules ──

    [Fact]
    public void Validate_VariantEmptyKey_HasError()
    {
        IReadOnlyList<CreateExperimentVariantInput> variants =
        [
            new("", "Control", 50, null, true),
            new("treatment", "Treatment", 50, null, false),
        ];
        var cmd = ValidCommand() with { Variants = variants };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public void Validate_VariantEmptyName_HasError()
    {
        IReadOnlyList<CreateExperimentVariantInput> variants =
        [
            new("control", "", 50, null, true),
            new("treatment", "Treatment", 50, null, false),
        ];
        var cmd = ValidCommand() with { Variants = variants };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public void Validate_VariantKeyTooLong_HasError()
    {
        IReadOnlyList<CreateExperimentVariantInput> variants =
        [
            new(new string('a', 101), "Control", 50, null, true),
            new("treatment", "Treatment", 50, null, false),
        ];
        var cmd = ValidCommand() with { Variants = variants };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public void Validate_VariantNameTooLong_HasError()
    {
        IReadOnlyList<CreateExperimentVariantInput> variants =
        [
            new("control", new string('x', 201), 50, null, true),
            new("treatment", "Treatment", 50, null, false),
        ];
        var cmd = ValidCommand() with { Variants = variants };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public void Validate_VariantAllocationNegative_HasError()
    {
        IReadOnlyList<CreateExperimentVariantInput> variants =
        [
            new("control", "Control", -10, null, true),
            new("treatment", "Treatment", 110, null, false),
        ];
        var cmd = ValidCommand() with { Variants = variants };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public void Validate_VariantAllocationOver100_HasError()
    {
        IReadOnlyList<CreateExperimentVariantInput> variants =
        [
            new("control", "Control", 101, null, true),
            new("treatment", "Treatment", -1, null, false),
        ];
        var cmd = ValidCommand() with { Variants = variants };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveAnyValidationError();
    }
}
