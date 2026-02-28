using FluentAssertions;
using FluentValidation.TestHelper;
using SwiggyClone.Application.Features.Auth.Commands;

namespace SwiggyClone.UnitTests.Validators.Auth;

public sealed class LoginByEmailCommandValidatorTests
{
    private readonly LoginByEmailCommandValidator _validator = new();

    private static LoginByEmailCommand ValidCommand() =>
        new("test@example.com", "P@ssw0rd!");

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
        var cmd = ValidCommand() with { Email = "notvalid" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_EmptyPassword_HasError()
    {
        var cmd = ValidCommand() with { Password = "" };
        var result = _validator.TestValidate(cmd);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}
