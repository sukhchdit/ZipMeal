using FluentAssertions;
using FluentValidation.TestHelper;
using SwiggyClone.Application.Features.Auth.Commands;

namespace SwiggyClone.UnitTests.Validators.Auth;

public sealed class RegisterByEmailCommandValidatorTests
{
    private readonly RegisterByEmailCommandValidator _validator = new();

    private static RegisterByEmailCommand ValidCommand() =>
        new("test@example.com", "P@ssw0rd!", "Test User", "+919876543210");

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        var result = _validator.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyEmail_HasError()
    {
        var cmd = ValidCommand() with { Email = "" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_InvalidEmail_HasError()
    {
        var cmd = ValidCommand() with { Email = "notanemail" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_ShortPassword_HasError()
    {
        var cmd = ValidCommand() with { Password = "P@1a" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must be at least 8 characters.");
    }

    [Fact]
    public void Validate_NoUppercase_HasError()
    {
        var cmd = ValidCommand() with { Password = "p@ssw0rd!" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one uppercase letter.");
    }

    [Fact]
    public void Validate_NoDigit_HasError()
    {
        var cmd = ValidCommand() with { Password = "P@ssword!" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one digit.");
    }

    [Fact]
    public void Validate_NoSpecialChar_HasError()
    {
        var cmd = ValidCommand() with { Password = "Passw0rdd" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one special character.");
    }

    [Fact]
    public void Validate_EmptyPhone_HasError()
    {
        var cmd = ValidCommand() with { PhoneNumber = "" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Fact]
    public void Validate_InvalidPhone_HasError()
    {
        var cmd = ValidCommand() with { PhoneNumber = "123" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("Phone number must be in E.164 format.");
    }

    [Fact]
    public void Validate_EmptyFullName_HasError()
    {
        var cmd = ValidCommand() with { FullName = "" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.FullName);
    }

    [Fact]
    public void Validate_EmptyPassword_HasError()
    {
        var cmd = ValidCommand() with { Password = "" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Validate_NoLowercase_HasError()
    {
        var cmd = ValidCommand() with { Password = "P@SSW0RD!" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must contain at least one lowercase letter.");
    }

    [Fact]
    public void Validate_ValidE164Phone_HasNoError()
    {
        var cmd = ValidCommand() with { PhoneNumber = "+14155552671" };
        var result = _validator.TestValidate(cmd);
        result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }
}
