using FluentAssertions;
using SwiggyClone.Application.Common.Helpers;

namespace SwiggyClone.UnitTests.Application.Helpers;

public sealed class SlugHelperTests
{
    [Fact]
    public void GenerateSlug_SimpleName_ReturnsLowercaseSlug()
    {
        SlugHelper.GenerateSlug("Pizza Palace").Should().Be("pizza-palace");
    }

    [Fact]
    public void GenerateSlug_Spaces_ReplacedWithHyphens()
    {
        SlugHelper.GenerateSlug("My Great Restaurant").Should().Be("my-great-restaurant");
    }

    [Fact]
    public void GenerateSlug_SpecialChars_Removed()
    {
        SlugHelper.GenerateSlug("Café & Bar!").Should().Be("cafe-bar");
    }

    [Fact]
    public void GenerateSlug_Diacritics_Removed()
    {
        SlugHelper.GenerateSlug("Résumé Café").Should().Be("resume-cafe");
    }

    [Fact]
    public void GenerateSlug_MultipleHyphens_Collapsed()
    {
        SlugHelper.GenerateSlug("Hello   World---Test").Should().Be("hello-world-test");
    }

    [Fact]
    public void GenerateSlug_LeadingTrailingSpaces_Trimmed()
    {
        SlugHelper.GenerateSlug("  Hello World  ").Should().Be("hello-world");
    }

    [Fact]
    public void GenerateSlug_UpperCase_Lowered()
    {
        SlugHelper.GenerateSlug("UPPER CASE NAME").Should().Be("upper-case-name");
    }

    [Fact]
    public void AppendSuffix_AddsSuffixCorrectly()
    {
        SlugHelper.AppendSuffix("pizza-palace", 2).Should().Be("pizza-palace-2");
    }
}
