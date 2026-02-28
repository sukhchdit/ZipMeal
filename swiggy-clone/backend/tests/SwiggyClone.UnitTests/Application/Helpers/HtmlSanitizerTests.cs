using FluentAssertions;
using SwiggyClone.Application.Common.Helpers;

namespace SwiggyClone.UnitTests.Application.Helpers;

public sealed class HtmlSanitizerTests
{
    [Fact]
    public void Sanitize_Null_ReturnsEmpty()
    {
        HtmlSanitizer.Sanitize(null).Should().BeEmpty();
    }

    [Fact]
    public void Sanitize_EmptyString_ReturnsEmpty()
    {
        HtmlSanitizer.Sanitize("  ").Should().BeEmpty();
    }

    [Fact]
    public void Sanitize_PlainText_ReturnsSameText()
    {
        HtmlSanitizer.Sanitize("Hello World").Should().Be("Hello World");
    }

    [Fact]
    public void Sanitize_ScriptTag_StripsTag()
    {
        HtmlSanitizer.Sanitize("<script>alert('xss')</script>").Should().Be("alert('xss')");
    }

    [Fact]
    public void Sanitize_NestedTags_StripsAllTags()
    {
        HtmlSanitizer.Sanitize("<div><p>Hello</p></div>").Should().Be("Hello");
    }

    [Fact]
    public void Sanitize_EncodedScriptTag_StripsAfterDecode()
    {
        var input = "&lt;script&gt;alert('xss')&lt;/script&gt;";

        var result = HtmlSanitizer.Sanitize(input);

        result.Should().Be("alert('xss')");
    }

    [Fact]
    public void Sanitize_HtmlEntities_Decoded()
    {
        HtmlSanitizer.Sanitize("5 &gt; 3 &amp; 2 &lt; 4").Should().Be("5 > 3 & 2 < 4");
    }

    [Fact]
    public void Sanitize_MixedContent_StripsTagsKeepsText()
    {
        HtmlSanitizer.Sanitize("Hello <b>World</b>!").Should().Be("Hello World!");
    }
}
